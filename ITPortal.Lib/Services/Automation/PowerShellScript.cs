using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Parameter;
using ITPortal.Lib.Utils;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;

namespace ITPortal.Lib.Services.Automation;

public class PowerShellScript
{
    public string? Content { get; private set; }
    public string? FilePath { get; private set; }
    public string? Name { get; private set; }
    public bool Loaded { get; private set; }
    public PSParameterList? Parameters { get; private set; }

    private readonly IOutputStreamService<PSMessage, PSStream> _outputStreamService;
    private readonly InitialSessionState _initialsessionState;

    public PowerShellScript(IOutputStreamService<PSMessage, PSStream> outputStreamService)
    {
        _outputStreamService = outputStreamService;
        // CreateDefault() only loads the commands necessary to host PowerShell, CreateDefault2() loads all available commands
        _initialsessionState = InitialSessionState.CreateDefault();
        // Limit script execution to one thread
        _initialsessionState.ApartmentState = ApartmentState.STA;
        // Set execution policy of the PS session
        _initialsessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
    }

    public PowerShellScript(IOutputStreamService<PSMessage, PSStream> outputStreamService, string filePath) : this(outputStreamService)
    {
        Load(filePath);
    }

    public bool Load(string filePath)
    {
        FilePath = filePath;

        return LoadScriptBlock(FilePath);
    }

    public bool Refresh()
    {
        return FilePath != null && LoadScriptBlock(FilePath);
    }

    private bool LoadScriptBlock(string filePath)
    {
        Content = FileHandler.GetFileContent(filePath);
        ScriptBlockAst scriptAst = Parser.ParseInput(Content, out _, out ParseError[] errors);

        if (errors.Length != 0)
        {
            Content = null;
            return false;
        }
        Name = Path.GetFileName(filePath);
        Parameters = scriptAst.ParamBlock != null ? new PSParameterList(scriptAst.ParamBlock.Parameters) : new();
        Loaded = true;

        return true;
    }

    public async Task<PSDataCollection<PSObject>?> Invoke(CancellationToken cancellationToken)
    {
        if (!Loaded) return null;

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create(_initialsessionState);
            shell.AddScript(Content);
            Parameters?.Register(shell);

            PSDataCollection<PSObject> outputCollection = new();
            _outputStreamService.SubscribeToPowerShellStream(outputCollection, PSStream.Output);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Information, PSStream.Information);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Progress, PSStream.Progress);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Warning, PSStream.Warning);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Error, PSStream.Error);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            Task<PSDataCollection<PSObject>> shellTask = Task.Factory.FromAsync(
                shell.BeginInvoke<PSObject, PSObject>(null, outputCollection),
                shell.EndInvoke);

            PSDataCollection<PSObject> result = await shellTask.WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception e)
        {
            _outputStreamService.AddOutput(PSStream.Error, e.Message);

            return null;
        }
    }

    public override string? ToString()
    {
        return Content;
    }
}

using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script.Parameter;
using ITPortal.Lib.Utils;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;

namespace ITPortal.Lib.Services.Automation.Script;

public class PowerShellScript : AutomationScript
{
    private readonly InitialSessionState _initialsessionState;

    public PowershellParameterList? Parameters { get; protected set; }

    public PowerShellScript(IScriptOutputStreamService outputStreamService) : base(outputStreamService)
    {
        // CreateDefault() only loads the commands necessary to host PowerShell, CreateDefault2() loads all available commands
        _initialsessionState = InitialSessionState.CreateDefault();
        // Limit script execution to one thread
        _initialsessionState.ApartmentState = ApartmentState.STA;
        // Set execution policy of the PS session
        _initialsessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
    }

    public override bool LoadScript(string filePath)
    {
        Content = FileHandler.GetFileContent(filePath);
        ScriptBlockAst scriptAst = Parser.ParseInput(Content, out _, out ParseError[] errors);

        if (errors.Length != 0)
        {
            Content = null;
            return false;
        }
        Name = Path.GetFileName(filePath);

        if (scriptAst.ParamBlock != null)
        {
            Parameters = new PowershellParameterList(scriptAst.ParamBlock.Parameters);
        }
        else
        {
            Parameters = new PowershellParameterList();
        }
        Loaded = true;

        return true;
    }

    public override async Task InvokeAsync(CancellationToken cancellationToken, string cancellationMessage)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            OutputStreamService?.AddOutput(ScriptStreamType.Warning, cancellationMessage);
            return;
        }

        if (!Loaded)
        {
            throw new InvalidPowerShellStateException("Attempt to invoke a script that was not loaded");
        }

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create(_initialsessionState);
            shell.AddScript(Content);
            Parameters?.Register(shell);

            PSDataCollection<PSObject> outputCollection = new();

            if (OutputStreamService != null)
            {
                OutputStreamService.SubscribeToOutputStream(outputCollection, ScriptStreamType.Output);
                OutputStreamService.SubscribeToOutputStream(shell.Streams.Information, ScriptStreamType.Information);
                OutputStreamService.SubscribeToOutputStream(shell.Streams.Progress, ScriptStreamType.Progress);
                OutputStreamService.SubscribeToOutputStream(shell.Streams.Warning, ScriptStreamType.Warning);
                OutputStreamService.SubscribeToOutputStream(shell.Streams.Error, ScriptStreamType.Error);
            }

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            Task<PSDataCollection<PSObject>> shellTask = Task.Factory.FromAsync(
                shell.BeginInvoke<PSObject, PSObject>(null, outputCollection),
                shell.EndInvoke);

            PSDataCollection<PSObject> result = await shellTask.WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            OutputStreamService?.AddOutput(ScriptStreamType.Warning, cancellationMessage);
        }
        catch (Exception e)
        {
            OutputStreamService?.AddOutput(ScriptStreamType.Error, e.Message);
        }
    }

    public override string? ToString()
    {
        return Content;
    }
}

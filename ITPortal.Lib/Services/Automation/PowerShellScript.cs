using ITPortal.Lib.Utils;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation;

public class PowerShellScript
{
    private readonly IOutputStreamService<PSMessage, PSStream> _outputStreamService;

    public string? ScriptBlock { get; private set; }
    public string? FilePath { get; private set; }
    public bool Loaded { get; private set; }

    public Dictionary<string, PSParameter>? Parameters { get; private set; }

    public PowerShellScript(IOutputStreamService<PSMessage, PSStream> outputStreamService)
    {
        _outputStreamService = outputStreamService;
    }

    public PowerShellScript(IOutputStreamService<PSMessage, PSStream> outputStreamService, string filePath)
    {
        _outputStreamService = outputStreamService;

        Load(filePath);
    }

    public bool Load(string filePath)
    {
        FilePath = filePath;

        return LoadScriptBlock(FilePath);
    }

    public bool Refresh()
    {
        if (FilePath == null) return false;

        return LoadScriptBlock(FilePath);
    }

    private bool LoadScriptBlock(string filePath)
    {
        ScriptBlock = FileHandler.GetFileContent(filePath);

        ScriptBlockAst ast = Parser.ParseInput(
            ScriptBlock,
            out _,
            out ParseError[] errors
        );

        if (errors.Length != 0) return false;

        Parameters = new Dictionary<string, PSParameter>();

        if (ast.ParamBlock != null)
        {
            foreach (var p in ast.ParamBlock.Parameters)
            {
                Parameters.Add(p.Name.ToString(), new PSParameter(p.StaticType));
            }
        }

        Loaded = true;

        return true;
    }

    public async Task<PSDataCollection<PSObject>?> Invoke(CancellationToken cancellationToken)
    {
        if (!Loaded) return null;

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create();
            shell.AddScript(ScriptBlock);

            if (Parameters != null && Parameters.Any())
            {
                // TODO: Might need to wrap in try/catch
                shell.AddParameters(Parameters);
            }

            var outputCollection = new PSDataCollection<PSObject>();

            _outputStreamService.SubscribeToPowerShellStream(outputCollection, PSStream.Output);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Information, PSStream.Information);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Progress, PSStream.Progress);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Warning, PSStream.Warning);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Error, PSStream.Error);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            var shellTask = Task.Factory.FromAsync(
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

    public void SetArgument(string parameterName, object parameter)
    {
        Parameters?[parameterName].SetValue(parameter);
    }

    public override string? ToString()
    {
        return ScriptBlock;
    }
}

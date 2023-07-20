using ITPortal.Lib.Utils;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation;

public class PowerShellScript
{
    private readonly IOutputStreamService<PSMessage, PSStream> _outputStreamService;

    private string? _scriptBlock;
    private string? _filePath;
    private bool _loaded;

    private Dictionary<string, object?>? _parameters;
    private Dictionary<string, object?>? _arguments;

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
        _filePath = filePath;

        return LoadScriptBlock(_filePath);
    }

    public bool Refresh()
    {
        if (_filePath == null) return false;

        return LoadScriptBlock(_filePath);
    }

    private bool LoadScriptBlock(string filePath)
    {
        _scriptBlock = FileHandler.GetFileContent(filePath);

        var ast = Parser.ParseInput(
            _scriptBlock,
            out _,
            out ParseError[] errors
        );

        if (errors.Length != 0) return false;

        _parameters = new Dictionary<string, object?>();
        _arguments = new Dictionary<string, object?>();

        if (ast.ParamBlock != null)
        {
            foreach (var p in ast.ParamBlock.Parameters)
            {
                var name = p.Name.ToString();

                _parameters.Add(name, p.StaticType);
                _arguments.Add(name, null);
            }
        }

        _loaded = true;

        return true;
    }

    public void SetArgument(string parameterName, object argument)
    {
        if (_arguments == null) return;

        _arguments[parameterName] = argument;
    }

    public async Task<PSDataCollection<PSObject>?> Invoke(CancellationToken cancellationToken)
    {
        if (!_loaded) return null;

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create();
            shell.AddScript(_scriptBlock);

            if (_arguments != null && _arguments.Any())
            {
                // TODO: Might need to wrap in try/catch
                shell.AddParameters(_arguments);
            }

            var outputCollection = new PSDataCollection<PSObject>();

            _outputStreamService.SubscribeToPowerShellStream(outputCollection, PSStream.Output);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Information, PSStream.Information);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Progress, PSStream.Progress);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Warning, PSStream.Warning);
            _outputStreamService.SubscribeToPowerShellStream(shell.Streams.Error, PSStream.Error);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            var result = await Task.Factory.FromAsync(shell
                .BeginInvoke<PSObject, PSObject>(null, outputCollection), shell.EndInvoke)
                .WaitAsync(cancellationToken)
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
        return _scriptBlock;
    }

    public string? GetScriptBlock()
    {
        return _scriptBlock;
    }

    public string? GetFilePath()
    {
        return _filePath;
    }

    public Dictionary<string, object?>? GetParameters()
    {
        return _parameters;
    }

    public bool IsLoaded()
    {
        return _loaded;
    }
}

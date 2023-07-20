using ITPortal.Lib.Utils;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation;

public class PowerShellScript
{
    private readonly IOutputStreamService<PSMessage, PSStream> _outputStreamService;

    private string? _scriptBlock;
    private string? _filePath;
    private bool _loaded;

    private Dictionary<string, Type>? _parameterTypes;
    private Dictionary<string, object?>? _parameters;

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

        ScriptBlockAst ast = Parser.ParseInput(
            _scriptBlock,
            out _,
            out ParseError[] errors
        );

        if (errors.Length != 0) return false;

        _parameterTypes = new Dictionary<string, Type>();
        _parameters = new Dictionary<string, object?>();

        if (ast.ParamBlock != null)
        {
            foreach (var p in ast.ParamBlock.Parameters)
            {
                var name = p.Name.ToString();

                _parameterTypes.Add(name, p.StaticType);
                _parameters.Add(name, null);
            }
        }

        _loaded = true;

        return true;
    }

    public async Task<PSDataCollection<PSObject>?> Invoke(CancellationToken cancellationToken)
    {
        if (!_loaded) return null;

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create();
            shell.AddScript(_scriptBlock);

            if (_parameters != null && _parameters.Any())
            {
                // TODO: Might need to wrap in try/catch
                shell.AddParameters(_parameters);
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
        if (_parameterTypes == null) return;

        if (parameter.GetType() == _parameterTypes[parameterName])
        {
            _parameters?.Add(parameterName, parameter);
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

    public Dictionary<string, Type>? GetParameterTypes()
    {
        return _parameterTypes;
    }

    public bool IsLoaded()
    {
        return _loaded;
    }
}

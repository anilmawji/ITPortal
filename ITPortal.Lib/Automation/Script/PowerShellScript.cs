using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script.Parameter;
using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script;

public sealed class PowerShellScript : AutomationScript
{
    private static readonly InitialSessionState _initialPowerShellState;

    static PowerShellScript()
    {
        // CreateDefault2() only loads commands necessary to host PowerShell, CreateDefault() loads all built-in commands
        _initialPowerShellState = InitialSessionState.CreateDefault();
        _initialPowerShellState.ApartmentState = ApartmentState.MTA;
        _initialPowerShellState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
    }

    public PowerShellScript() { }

    [JsonConstructor]
    public PowerShellScript(string filePath, string fileName, string[] content, List<ScriptParameter> parameters)
        : base(filePath, fileName, content, parameters) { }

    public override bool LoadParameters()
    {
        if (!IsContentLoaded())
        {
            throw new InvalidOperationException("Attempt to load parameters of a script that has not been loaded");
        }

        ScriptBlockAst scriptAst = Parser.ParseInput(ContentString, out _, out ParseError[] errors);

        if (errors.Length != 0)
        {
            return false;
        }
        if (scriptAst.ParamBlock == null)
        {
            return true;
        }
        foreach (var parameterAst in scriptAst.ParamBlock.Parameters)
        {
            ScriptParameter parameter = new(
                parameterAst.Name.VariablePath.ToString(),
                parameterAst.StaticType,
                parameterAst.IsMandatory()
            );
            Parameters.Add(parameter);
        }
        return true;
    }

    public override ScriptOutputList NewScriptOutputList()
    {
        return new PowerShellScriptOutputList();
    }

    public override async Task<ScriptExecutionState> InvokeAsync(string deviceName, ScriptOutputList outputList,
        string cancellationMessage = DefaultCancellationMessage, CancellationToken cancellationToken = default)
    {
        if (!IsContentLoaded())
        {
            throw new InvalidOperationException("Attempt to invoke a script that has not been loaded");
        }

        // Check if a pre-cancelled token was given
        if (cancellationToken.IsCancellationRequested)
        {
            outputList.Add(cancellationMessage, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }

        using PowerShell shell = PowerShell.Create(_initialPowerShellState);

        try
        {
            shell.AddScript(ContentString);

            if (Parameters.Any())
            {
                RegisterParameters(shell);
            }
            PSDataCollection<PSObject> standardOutputStream = RegisterOutputStreams(shell, outputList);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            await Task.Factory.FromAsync(shell.BeginInvoke<PSObject, PSObject>(null, standardOutputStream), shell.EndInvoke)
                .WaitAsync(cancellationToken);

            return shell.HadErrors || outputList.HasErrorMessages() ? ScriptExecutionState.Error : ScriptExecutionState.Success;
        }
        catch (OperationCanceledException)
        {
            outputList.Add(cancellationMessage, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }
        catch (PipelineStoppedException e)
        {
            outputList.Add(e.Message, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }
        catch (Exception e)
        {
            outputList.Add(e.Message, ScriptOutputStreamType.Error);

            return ScriptExecutionState.Error;
        }
    }

    private void RegisterParameters(PowerShell shell)
    {
        foreach (ScriptParameter parameter in Parameters)
        {
            shell.AddParameter(parameter.Name, parameter.Value);
        }
    }

    private static PSDataCollection<PSObject> RegisterOutputStreams(PowerShell shell, ScriptOutputList outputList)
    {
        PSDataCollection<PSObject> standardOutputStream = new();

        outputList.SubscribeToOutputStream(standardOutputStream, ScriptOutputStreamType.Standard);
        outputList.SubscribeToOutputStream(shell.Streams.Information, ScriptOutputStreamType.Information);
        outputList.SubscribeToOutputStream(shell.Streams.Progress, ScriptOutputStreamType.Progress);
        outputList.SubscribeToOutputStream(shell.Streams.Warning, ScriptOutputStreamType.Warning);

        if (outputList is PowerShellScriptOutputList psOutputList)
        {
            // Provide more detailed PowerShell-specific errors
            psOutputList.SubscribeToErrorOutputStream(shell.Streams.Error);
        }
        else
        {
            outputList.SubscribeToOutputStream(shell.Streams.Error, ScriptOutputStreamType.Error);
        }
        return standardOutputStream;
    }
}

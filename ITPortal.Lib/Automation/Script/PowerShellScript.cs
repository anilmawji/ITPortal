using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script.Parameter;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;

namespace ITPortal.Lib.Automation.Script;

public sealed class PowerShellScript : AutomationScript
{
    private readonly InitialSessionState _initialPowerShellState;

    public PowerShellScript()
    {
        _initialPowerShellState = NewInitialSessionState();
    }

    public PowerShellScript(string filePath) : base(filePath)
    {
        _initialPowerShellState = NewInitialSessionState();
    }

    public PowerShellScript(string filePath, string deviceName) : base(filePath, deviceName)
    {
        _initialPowerShellState = NewInitialSessionState();
    }

    private static InitialSessionState NewInitialSessionState()
    {
        // CreateDefault() only loads the commands necessary to host PowerShell, CreateDefault2() loads all available commands
        InitialSessionState initialPowerShellState = InitialSessionState.CreateDefault();
        // Limit script execution to one thread
        initialPowerShellState.ApartmentState = ApartmentState.MTA;
        // Set execution policy of the PS session
        initialPowerShellState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;

        return initialPowerShellState;
    }

    public override bool LoadParameters()
    {
        if (!IsLoaded())
        {
            throw new InvalidOperationException("Cannot load parameters on an unloaded script");
        }

        ScriptBlockAst scriptAst = Parser.ParseInput(ContentString, out _, out ParseError[] errors);

        if (errors.Length != 0)
        {
            LoadState = ScriptLoadState.Failed;
            return false;
        }

        if (scriptAst.ParamBlock != null)
        {
            Parameters = new PowershellParameterList(scriptAst.ParamBlock.Parameters);
        }
        else
        {
            Parameters = new PowershellParameterList();
        }

        return true;
    }

    public override async Task<ScriptExecutionState> InvokeAsync(string cancellationMessage, ScriptOutputList scriptOutput, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            scriptOutput.Add(cancellationMessage, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }

        if (!IsLoaded())
        {
            throw new InvalidOperationException("Attempt to invoke a script that was not loaded");
        }

        try
        {
            // "using" relies on compiler to dispose of shell when method is popped from call stack
            using PowerShell shell = PowerShell.Create(_initialPowerShellState);
            shell.AddScript(ContentString);

            if (Parameters.Any())
            {
                RegisterParameters(shell);
            }
            PSDataCollection<PSObject> standardOutputStream = RegisterOutputStreams(shell, scriptOutput);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            Task<PSDataCollection<PSObject>> shellTask = Task.Factory.FromAsync(
                shell.BeginInvoke<PSObject, PSObject>(null, standardOutputStream),
                shell.EndInvoke);

            await shellTask.WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            return ScriptExecutionState.Success;
        }
        catch (OperationCanceledException)
        {
            scriptOutput.Add(cancellationMessage, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }
        catch (Exception e)
        {
            scriptOutput.Add(e.Message, ScriptOutputStreamType.Error);

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

    private static PSDataCollection<PSObject> RegisterOutputStreams(PowerShell shell, ScriptOutputList scriptOutput)
    {
        PSDataCollection<PSObject> standardOutputStream = new();

        scriptOutput.SubscribeToOutputStream(standardOutputStream, ScriptOutputStreamType.Standard);
        scriptOutput.SubscribeToOutputStream(shell.Streams.Information, ScriptOutputStreamType.Information);
        scriptOutput.SubscribeToOutputStream(shell.Streams.Progress, ScriptOutputStreamType.Progress);
        scriptOutput.SubscribeToOutputStream(shell.Streams.Warning, ScriptOutputStreamType.Warning);
        scriptOutput.SubscribeToOutputStream(shell.Streams.Error, ScriptOutputStreamType.Error);

        return standardOutputStream;
    }
}

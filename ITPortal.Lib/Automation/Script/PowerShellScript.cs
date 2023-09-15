﻿using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script.Parameter;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Text.Json.Serialization;

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

    [JsonConstructor]
    public PowerShellScript(string filePath, string fileName, string[] content, List<ScriptParameter> parameters)
        : base(filePath, fileName, content, parameters)
    {
        _initialPowerShellState = NewInitialSessionState();
    }

    private static InitialSessionState NewInitialSessionState()
    {
        // CreateDefault2() only loads the commands necessary to host PowerShell, CreateDefault() loads all build-in commands
        InitialSessionState initialPowerShellState = InitialSessionState.CreateDefault();
        // Allow multiple threads in the PS session
        initialPowerShellState.ApartmentState = ApartmentState.MTA;
        // Set execution policy of the PS session
        initialPowerShellState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;

        return initialPowerShellState;
    }

    public override bool LoadParameters()
    {
        if (!IsContentLoaded())
        {
            throw new InvalidOperationException("Cannot load parameters on an unloaded script");
        }

        ScriptBlockAst scriptAst = Parser.ParseInput(ContentString, out _, out ParseError[] errors);

        if (errors.Length != 0)
        {
            return false;
        }
        if (scriptAst.ParamBlock != null)
        {
            foreach (ParameterAst parameter in scriptAst.ParamBlock.Parameters)
            {
                AddParameter(parameter);
            }
        }
        return true;
    }

    public void AddParameter(ParameterAst parameter)
    {
        Parameters.Add(new ScriptParameter(parameter.Name.VariablePath.ToString(), parameter.StaticType, parameter.IsMandatory()));
    }

    public override ScriptOutputList NewScriptOutputList()
    {
        return new PowerShellScriptOutputList();
    }

    public override async Task<ScriptExecutionState> InvokeAsync(string deviceName, ScriptOutputList outputList,
        string cancellationMessage = DefaultCancellationMessage, CancellationToken cancellationToken = default)
    {
        // Check if a pre-cancelled token was given
        if (cancellationToken.IsCancellationRequested)
        {
            outputList.Add(cancellationMessage, ScriptOutputStreamType.Warning);

            return ScriptExecutionState.Stopped;
        }
        if (!IsContentLoaded())
        {
            throw new InvalidOperationException("Cannot invoke a script that has not been loaded");
        }
        try
        {
            // "using" relies on compiler to dispose of shell when try-catch block is exited
            using PowerShell shell = PowerShell.Create(_initialPowerShellState);
            shell.AddScript(ContentString);

            if (Parameters.Any())
            {
                RegisterParameters(shell);
            }
            PSDataCollection<PSObject> standardOutputStream = RegisterOutputStreams(shell, outputList);

            // Use Task.Factory to opt for the newer async/await keywords
            // Moves away from the old IAsyncResult functionality still used by the PowerShell API
            Task<PSDataCollection<PSObject>> shellTask = Task.Factory.FromAsync(
                shell.BeginInvoke<PSObject, PSObject>(null, standardOutputStream),
                shell.EndInvoke);

            await shellTask.WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (shell.HadErrors)
            {
                return ScriptExecutionState.Error;
            }
            return ScriptExecutionState.Success;
        }
        catch (OperationCanceledException)
        {
            outputList.Add(cancellationMessage, ScriptOutputStreamType.Warning);

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

﻿using System.Management.Automation;
using System.Text.Json.Serialization;

namespace ScriptProfiler.Lib.Automation.Output;

public class PowerShellScriptOutputList : ScriptOutputList
{
    public PowerShellScriptOutputList() : base() { }

    [JsonConstructor]
    public PowerShellScriptOutputList(List<ScriptOutputMessage> output, Dictionary<ScriptOutputStreamType, int> streamLineCounts)
        : base(output, streamLineCounts) { }

    public override void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType)
    {
        var psStream = (PSDataCollection<T>)stream;

        psStream.DataAdded += (object? sender, DataAddedEventArgs e) =>
        {
            string? message = psStream[e.Index]?.ToString();
            Add(message, streamType);
        };
    }

    public void SubscribeToErrorOutputStream(PSDataCollection<ErrorRecord> stream)
    {
        stream.DataAdded += (object? sender, DataAddedEventArgs e) =>
        {
            ErrorRecord error = stream[e.Index];
            Add(error.Exception.GetBaseException().Message, ScriptOutputStreamType.Error);
            Add("[CategoryInfo]: " + error.CategoryInfo.ToString(), ScriptOutputStreamType.Error);
            Add("[FullyQualifiedErrorId]: " + error.FullyQualifiedErrorId, ScriptOutputStreamType.Error);

            if (error.ErrorDetails != null)
            {
                Add("[Recommended Action]: " + error.ErrorDetails.RecommendedAction, ScriptOutputStreamType.Error);
            }
        };
    }
}

using ITPortal.Lib.Utility;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ITPortal.Lib.Services;

public class JsonSerializationService<T> : ISerializationService<T>
{
    private readonly SerializationSettings _serializationSettings;
    private readonly string _saveDirPath;

    public JsonTypeInfo<T> TypeInfo { get; private set; }

    // TODO: randomize file name of JSON files
    public JsonSerializationService(IOptions<SerializationSettings> options, JsonTypeInfo<T> typeInfo, string parentSaveDirPath)
    {
        _serializationSettings = options.Value;
        TypeInfo = typeInfo;
        _saveDirPath = Path.Combine(parentSaveDirPath, typeof(T).Name);
    }

    public string GetFilePath(string fileName)
    {
        return Path.Combine(_saveDirPath, fileName + ".json");
    }

    private void TryLogEvent(LogEvent eventType, string message)
    {
        if (_serializationSettings.EnableLogging)
        {
            Logger.AddMessage(eventType, message);
        }
    }

    public void LoadFromSaveDirectory(ICollection<T> objList)
    {
        DirectoryInfo info = Directory.CreateDirectory(_saveDirPath);

        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(_saveDirPath);

        foreach (string path in filePaths)
        {
            T? result = LoadFromFile(path);

            if (result == null) continue;

            objList.Add(result);
        }
    }

    public T? LoadFromFile(string jsonFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

        T? result;

        try
        {
            result = JsonSerializer.Deserialize(File.ReadAllText(jsonFilePath), TypeInfo);
        }
        catch (JsonException e)
        {
            TryLogEvent(LogEvent.Error, $"Failed to deserialize file \'{fileName}\': {e.Message}");

            return default;
        }

        if (Path.GetDirectoryName(jsonFilePath) != _saveDirPath)
        {
            try
            {
                File.Copy(jsonFilePath, Path.Combine(_saveDirPath, Path.GetFileName(jsonFilePath)));
            }
            catch (Exception e)
            {
                TryLogEvent(LogEvent.Warning, $"Failed to copy file to \'{_saveDirPath}\': {e.Message}");
            }
        }

        return result;
    }

    public bool TryCreateFile(T obj, string filePath)
    {
        try
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(obj, TypeInfo));

            return true;
        }
        catch (Exception e)
        {
            TryLogEvent(LogEvent.Error, $"Failed to create file \'{filePath}\': {e.Message}");

            return false;
        }
    }

    public bool TryDeleteFile(string fileName)
    {
        string filePath = GetFilePath(fileName);

        try
        {
            File.Delete(filePath);

            return true;
        }
        catch (Exception e)
        {
            TryLogEvent(LogEvent.Error, $"Failed to delete file \'{filePath}\': {e.Message}");

            return false;
        }
    }
}

using ITPortal.Lib.Utility;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ITPortal.Lib.Services;

public class JsonSerializationService<T> : IObjectSerializationService<T>
{
    public string SaveFolderPath { get; private set; }
    public bool LoggingEnabled { get; private set; }
    public JsonTypeInfo<T> TypeInfo { get; private set; }

    // TODO: randomize file name of JSON files
    public JsonSerializationService(string saveFolderPath, JsonTypeInfo<T> typeInfo, bool loggingEnabled)
    {
        SaveFolderPath = saveFolderPath;
        TypeInfo = typeInfo;
        LoggingEnabled = loggingEnabled;
    }

    public string GetFilePath(string fileName)
    {
        return Path.Combine(SaveFolderPath, Guid.NewGuid() + ".json");
    }

    private void TryLogEvent(LogEvent eventType, string message)
    {
        if (LoggingEnabled)
        {
            Logger.AddMessage(eventType, message);
        }
    }

    public void LoadFromSaveFolder(ICollection<T> objList)
    {
        DirectoryInfo info = Directory.CreateDirectory(SaveFolderPath);

        // Jobs folder has just been created; no jobs to load
        if (!info.Exists)
        {
            return;
        }

        IEnumerable<string> filePaths = Directory.EnumerateFiles(SaveFolderPath);

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

        if (Path.GetDirectoryName(jsonFilePath) != SaveFolderPath)
        {
            try
            {
                File.Copy(jsonFilePath, Path.Combine(SaveFolderPath, Path.GetFileName(jsonFilePath)));
            }
            catch (Exception e)
            {
                TryLogEvent(LogEvent.Warning, $"Failed to copy file to \'{SaveFolderPath}\': {e.Message}");
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

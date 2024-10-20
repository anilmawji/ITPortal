namespace ScriptProfiler.Lib.Services;

public interface ISerializationService<T>
{
    public string GetFilePath(string fileName);

    public void LoadFromSaveDirectory(ICollection<T> objList);

    public T? LoadFromFile(string filePath);

    public bool TryCreateFile(T obj, string filePath);

    public bool TryDeleteFile(string fileName);
}

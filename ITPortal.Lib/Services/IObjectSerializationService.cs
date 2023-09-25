namespace ITPortal.Lib.Services;

public interface IObjectSerializationService<T>
{
    public string SaveFolderPath { get; }

    public string GetFilePath(string fileName);
    public IEnumerable<T> LoadFromSaveFolder();
    public T? LoadFromFile(string filePath);
    public bool TryCreateFile(T obj, string filePath);
    public bool TryDeleteFile(string fileName);
}

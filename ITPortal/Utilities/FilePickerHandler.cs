namespace ITPortal.Utilities;

public sealed class FilePickerHandler
{
    public static async Task<FileResult> PickFile(PickOptions options)
    {
        try
        {
            // Prompt user to pick file
            FileResult result = await FilePicker.PickAsync(options);

            if (result == null)
            {
                return null;
            }

            foreach (string fileType in options.FileTypes.Value)
            {
                if (result.FileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

namespace ITPortal.Utils;

public class FilePickerHandler
{
    public static async Task<FileResult> PickFile(PickOptions options)
    {
        try
        {
            // Prompt user to pick file
            var result = await FilePicker.PickAsync(options);

            if (result != null)
            {
                foreach (var fileType in options.FileTypes.Value)
                {
                    if (result.FileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
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

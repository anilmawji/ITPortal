namespace ITPortal.Lib.Services.Core;

public interface INavigationService
{
    public void NavigateTo(string url);

    public void NavigateBack();

    public IReadOnlyList<string> GetHistory();
}

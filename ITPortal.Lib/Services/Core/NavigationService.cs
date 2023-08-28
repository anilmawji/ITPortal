using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace ITPortal.Lib.Services.Core;

public sealed class NavigationService : INavigationService, IDisposable
{
    public const int MaxHistorySize = 16;

    private readonly NavigationManager _navigationManager;
    private readonly List<string> _history = new();

    public NavigationService(NavigationManager navigationManager) {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;

        _history.Add(_navigationManager.Uri);
    }

    public bool CanNavigateBack => _history.Count >= 2;

    public bool IsHistoryFull => _history.Count == MaxHistorySize;

    public void NavigateTo(string url)
    {
        _navigationManager.NavigateTo(url);
    }

    public void NavigateBack()
    {
        if (!CanNavigateBack) return;

        string previousUrl = _history.Last();
        _navigationManager.NavigateTo(previousUrl);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (IsHistoryFull)
        {
            _history.RemoveAt(0);
        }
        _history.Add(e.Location);
    }

    public IReadOnlyList<string> GetHistory()
    {
        return _history.AsReadOnly();
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}

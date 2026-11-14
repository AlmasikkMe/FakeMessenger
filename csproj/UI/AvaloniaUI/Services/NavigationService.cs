using Avalonia.Controls;

namespace FakeMessenger.UI.AvaloniaUI.Services;

public class NavigationService
{
    public event EventHandler<Page>? Navigation; 
    public void NavigateTo(Page page, object? sender = null)
    {
        Navigation?.Invoke(sender, page);
    }
    public void GoBack() { throw new NotImplementedException(); }
}
    
using Avalonia.Controls;

namespace FakeMessenger.UI.AvaloniaUI.Services;

public class NavigationService
{
    public event EventHandler<Page>? Navigation; 
    private Page? CurrentPage;
    private Page? LastPage;

    public void NavigateTo(Page page, object? sender = null)
    {
        LastPage = CurrentPage;
        CurrentPage = page;
        Navigation?.Invoke(sender, page);
    }
    public void GoBack(object? sender = null)
    {
        if (LastPage is not null)
            Navigation?.Invoke(sender, LastPage);
    }
}
    
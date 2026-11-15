using Avalonia;
using Avalonia.Controls;
using FakeMessenger.UI.AvaloniaUI.Pages;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        (Application.Current as App)?.NavigationService?.Navigation += NavigateTo; 
        (Application.Current as App)?.NavigationService?.NavigateTo(new ChatSelectionPage());
    }

    public void NavigateTo(object? sender, Page page)
    {
        MainFrame.Content = page;
    }
}
using Avalonia.Controls;
using FakeMessenger.UI.AvaloniaUI.Pages;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Content = new ChatSelectionPage();
    }

    public void NavigateTo(Page page)
    {
        MainFrame.Content = page;
    }
}
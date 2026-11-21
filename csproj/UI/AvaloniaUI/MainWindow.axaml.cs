using Avalonia;
using Avalonia.Controls;
using FakeMessenger.UI.AvaloniaUI.Pages;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class MainWindow : Window
{
    public MainWindow(INavigation? navigation = null)
    {
        InitializeComponent();
        navigation ??= new NavigationPage();
        Content = navigation;
        navigation.PushAsync(new ChatSelectionPage());  
    }
}
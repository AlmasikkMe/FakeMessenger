using System.Windows;
using System.Windows.Controls;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class CreateChatPage : Page
{
    public CreateChatPage()
    {
        InitializeComponent();

        Loaded += CreateChatPage_Loaded;
    }

    private void CreateChatPage_Loaded(object sender, RoutedEventArgs e)
    {
        ChatNameBox.Focus();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        GoBack();
    }

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        string name = ChatNameBox.Text.Trim();

        if (name.Length == 0)
        {
            ChatNameBox.Focus();
            return;
        }

        GoBack();
    }

    private void GoBack()
    {
        if (NavigationService is not null && NavigationService.CanGoBack)
        {
            NavigationService.GoBack();
            return;
        }

        NavigationService?.Navigate(new ChatsPage());
    }
}
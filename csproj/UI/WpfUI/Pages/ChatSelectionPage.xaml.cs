using System.Windows;
using System.Windows.Controls;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatSelectionPage : Page
{
    public ChatSelectionPage()
    {
        InitializeComponent();
        Loaded += ChatSelectionPage_Loaded;
    }

    private void ChatSelectionPage_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshChats();
    }

    private void RefreshChats()
    {
        ChatsListView.ItemsSource = App.AppService.Chats;
    }

    private void ChatsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ChatsListView.SelectedItem is Chat selectedChat)
        {
            NavigationService.Navigate(new ChatPage(selectedChat));
        }
    }

    private void CreateChat_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new CreateChatPage());
    }

    private void CreateContact_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new CreateContactPage());
    }
}
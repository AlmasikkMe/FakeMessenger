using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FakeMessenger.Core.Entities;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI.Pages;

public partial class ChatSelectionPage : ContentPage
{
    private IAppService? _appService;
    private INavigation? _navigation;

    public ChatSelectionPage()  
    {
        InitializeComponent();
        _navigation = (Application.Current as App)?.Navigation;
        _appService = (Application.Current as App)?.AppService;
        ChatsListBox.ItemsSource = _appService?.Chats;
    }

    private void ChatsListView_DoubleTapped(object sender, RoutedEventArgs e)
    {
        if (ChatsListBox.SelectedItem is Chat selectedChat)
        {
            _navigation?.PushAsync(new ChatPage(selectedChat));
        }
    }

    private void CreateChat_Click(object sender, RoutedEventArgs e)
    {
        _navigation?.PushAsync(new CreateChatPage());
    }

    private void CreateContact_Click(object sender, RoutedEventArgs e)
    {
        _navigation?.PushAsync(new CreateContactPage());
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _appService?.Save();
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        try { _appService?.Load(); }
        catch (Exception ex)
        { 
            // MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChatDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            if (menuItem.DataContext is Chat currentChat)
            {
                _appService?.RemoveChat(currentChat);
            }
        }
    }
}
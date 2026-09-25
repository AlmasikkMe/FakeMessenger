using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatPage : Page
{
    private readonly Chat _chat;

    public ChatPage(Chat chat)
    {
        InitializeComponent();
        _chat = chat;
        MessageSenderComboBox.ItemsSource = chat.Members.Select(u => u.FullName);
        MessageSenderComboBox.SelectedIndex = 0;
        Loaded += ChatPage_Loaded;
    }

    private void ChatPage_Loaded(object sender, RoutedEventArgs e)
    {
        ChatTitleTextBlock.Text = _chat.Name;
        RefreshMessages();
    }

    private void RefreshMessages()
    {
        // Обновляем источник, чтобы подхватить новые сообщения
        MessagesListBox.ItemsSource = null;
        MessagesListBox.ItemsSource = _chat.Messages;
        if (MessagesListBox.Items.Count > 0)
        {
            MessagesListBox.ScrollIntoView(MessagesListBox.Items[MessagesListBox.Items.Count - 1]);
        }
    }

    private void Send_Click(object sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SendMessage();
            e.Handled = true;
        }
    }

    private void SendMessage()
    {
        User sender = _chat.Members.First(u => u.FullName == MessageSenderComboBox.SelectedItem.ToString());
        string text = MessageTextBox.Text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        App.AppService.SendMessage(sender, _chat, text);
        MessageTextBox.Clear();
        RefreshMessages();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void MessageCopy_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            if (menuItem.DataContext is Message currentMessage)
            {
                Clipboard.SetText(currentMessage.Text); 
            }
        }
    }

    private void MessageDelete_Click(object sender, RoutedEventArgs e)
    {
        
        if (sender is MenuItem menuItem)
        {
            if (menuItem.DataContext is Message currentMessage)
            {
                _chat.DeleteMessage(currentMessage);
                RefreshMessages();
            }
        }
    }
}
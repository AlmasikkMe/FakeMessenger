using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatPage : Page
{
    private readonly Chat _chat;
    private Message? _editing;

    public ChatPage(Chat chat)
    {
        InitializeComponent();
        
        MessageDateTimePicker.Value = DateTime.Now;
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
        if (text.IsWhiteSpace())
        {
            return;
        }

        if (_editing is null)
        {
            App.AppService.SendMessage(sender, _chat, text, dateTime: MessageDateTimePicker.Value);
        }
        else
        {
            _editing.Text = text;
            _editing.Sender = sender;
            _editing.DateTime = MessageDateTimePicker.Value ?? DateTime.Now;
            _editing = null;
        }

        MessageTextBox.Clear();
        RefreshMessages();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void MessageCopy_Click(object sender, RoutedEventArgs e)
    {
        StringBuilder sb = new();
        foreach (Message message in MessagesListBox.SelectedItems)
            sb.AppendLine(message.Text);

        try
        {
            Clipboard.SetText(sb.ToString());
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            MessageBox.Show("Не удалось получить доступ к буферу обмена. Попробуйте еще раз.");
        }

    }

    private void MessageEdit_Click(object sender, RoutedEventArgs e)
    {
        if (MessagesListBox.SelectedItem is Message message)
        {
            _editing = message;
            MessageTextBox.Text = message.Text;
            MessageSenderComboBox.SelectedItem = message.Sender.FirstName;
            MessageDateTimePicker.Value = message.DateTime;
        }
    }

    private void MessageDelete_Click(object sender, RoutedEventArgs e)
    {
        foreach (Message message in MessagesListBox.SelectedItems)
            _chat.DeleteMessage(message);
        RefreshMessages();
    }
}
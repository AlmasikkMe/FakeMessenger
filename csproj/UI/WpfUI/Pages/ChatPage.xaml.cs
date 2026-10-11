using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FakeMessenger.Core.Entities;
using FakeMessenger.UI.WpfUI.Services;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatPage : Page
{
    public ObservableCollection<Message> Messages { get; set; }
    private readonly Chat _chat;
    private Message? _editing;

    public ChatPage(Chat chat)
    {
        _chat = chat;
        Messages = new(_chat.MessagesRepository.Get());

        InitializeComponent();

        DataContext = this;

        MessageDateTimePicker.Value = DateTime.Now;
        MessageSenderComboBox.ItemsSource = chat.MembersRepository.Get().Select(u => u.FullName);
        MessageSenderComboBox.SelectedIndex = 0;

        Loaded += ChatPage_Loaded;
        Unloaded += ChatPage_Unloaded;
    }

    private void ChatPage_Loaded(object sender, RoutedEventArgs e)
    {
        ChatTitleTextBlock.Text = _chat.Name;

        _chat.MessagesRepository.CollectionChanged -= MessagesRepository_CollectionChanged;
        _chat.MessagesRepository.CollectionChanged += MessagesRepository_CollectionChanged;
    }

    private void ChatPage_Unloaded(object sender, RoutedEventArgs e)
    {
        _chat.MessagesRepository.CollectionChanged -= MessagesRepository_CollectionChanged;
    }

    private void MessagesRepository_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
            MessengerService.UpdateObservableCollection(
                _chat.MessagesRepository.Get(), Messages, e
            )
        );
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
        User sender = _chat.MembersRepository.Get().First(u => u.FullName == MessageSenderComboBox.SelectedItem.ToString());
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
            _chat.MessagesRepository.Remove(message);
    }
}
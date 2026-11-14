using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using FakeMessenger.Core.Entities;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI.Pages;

public partial class ChatPage : ContentPage
{
    public ObservableCollection<Message> Messages { get; set; }
    private readonly Chat _chat;
    private Message? _editing;
    private IAppService? _appService;
    private NavigationService? _navigationService;

    public ChatPage()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += ChatPage_Loaded;
        Unloaded += ChatPage_Unloaded;
    }

    public ChatPage(Chat chat) : this()
    {
        _chat = chat;
        Messages = new(_chat.MessagesRepository.Get());

        // Теперь MessageSenderComboBox уже инициализирован благодаря : this()
        MessageSenderComboBox.ItemsSource = chat.MembersRepository.Get().Select(u => u.FullName);
        MessageSenderComboBox.SelectedIndex = 0;
    }


    private void ChatPage_Loaded(object? sender, RoutedEventArgs e)
    {
        ChatTitleTextBlock.Text = _chat.Name;

        _chat.MessagesRepository.CollectionChanged -= MessagesRepository_CollectionChanged;
        _chat.MessagesRepository.CollectionChanged += MessagesRepository_CollectionChanged;
    }

    private void ChatPage_Unloaded(object? sender, RoutedEventArgs e)
    {
        _chat.MessagesRepository.CollectionChanged -= MessagesRepository_CollectionChanged;
    }

    private void MessagesRepository_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current?.Dispatcher.Invoke(() =>
            MessengerService.UpdateObservableCollection(
                _chat.MessagesRepository.Get(), Messages, e
            )
        );
    }

    private void Send_Click(object? sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    private void MessageTextBox_KeyDown(object? sender, KeyEventArgs e)
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
            _appService.SendMessage(sender, _chat, text); //, dateTime: MessageDateTimePicker.Value);
        }
        else
        {
            _editing.Text = text;
            _editing.Sender = sender;
            // _editing.DateTime = MessageDateTimePicker.Value ?? DateTime.Now;
            _editing = null;
        }

        MessageTextBox.Clear();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _navigationService.GoBack();
    }

    private void MessageCopy_Click(object sender, RoutedEventArgs e)
    {
        StringBuilder sb = new();
        foreach (Message message in MessagesListBox.SelectedItems)
            sb.AppendLine(message.Text);

        Task.Run(() => ClipboardCopy(sb.ToString()));
    }

    private async Task ClipboardCopy(string text)
    {
        if (TopLevel.GetTopLevel(this)?.Clipboard is IClipboard clipboard)
            await clipboard.SetTextAsync(text);
    }

    private void MessageEdit_Click(object sender, RoutedEventArgs e)
    {
        if (MessagesListBox.SelectedItem is Message message)
        {
            _editing = message;
            MessageTextBox.Text = message.Text;
            MessageSenderComboBox.SelectedItem = message.Sender.FirstName;
            // MessageDateTimePicker.Value = message.DateTime;
        }
    }

    private void MessageDelete_Click(object sender, RoutedEventArgs e)
    {
        List<Message> selected = MessagesListBox.SelectedItems.Cast<Message>().ToList();

        foreach (Message message in selected)
            _chat.MessagesRepository.Remove(message);
    }
}
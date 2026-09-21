using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FakeMessenger.UI.WpfUI.Models;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatPage : Page
{
    private readonly ChatItem _chat;
    private readonly ObservableCollection<MessageItem> _messages;

    public ChatPage(ChatItem chat)
    {
        InitializeComponent();

        _chat = chat;

        HeaderTitle.Text = chat.Title;
        HeaderAvatar.Text = chat.AvatarText;

        _messages = new ObservableCollection<MessageItem>
        {
            new MessageItem("Привет! Ты уже посмотрел макеты?", "12:30", false),
            new MessageItem("Да, глянул утром. Выглядит неплохо", "12:32", true),
            new MessageItem("Есть пара замечаний по отступам в списке", "12:33", true),
            new MessageItem("Ок, скинь их, поправлю", "12:35", false),
            new MessageItem("И ещё: кнопка отправки слишком близко к краю", "12:36", false),
            new MessageItem("Понял, добавлю отступ 8 пикселей", "12:40", true),
            new MessageItem("Увидимся завтра в 10:00", "12:41", false)
        };

        MessagesList.ItemsSource = _messages;

        Loaded += ChatPage_Loaded;
    }

    private void ChatPage_Loaded(object sender, RoutedEventArgs e)
    {
        MessageBox.Focus();
        MessagesScroll.ScrollToEnd();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        GoBack();
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    private void MessageBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        SendMessage();
        e.Handled = true;
    }

    private void SendMessage()
    {
        string text = MessageBox.Text.Trim();

        if (text.Length == 0)
        {
            return;
        }

        _messages.Add(new MessageItem(text, DateTime.Now.ToString("HH:mm"), true));

        MessageBox.Clear();
        MessagesScroll.ScrollToEnd();
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
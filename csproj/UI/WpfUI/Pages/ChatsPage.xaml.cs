using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FakeMessenger.UI.WpfUI.Models;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class ChatsPage : Page
{
    private readonly List<ChatItem> _chats;

    public ChatsPage()
    {
        InitializeComponent();

        _chats = new List<ChatItem>
        {
            new ChatItem("Анна Смирнова", "Увидимся завтра в 10:00", "12:41", "А", 2),
            new ChatItem("Команда разработки", "Максим: залил новую сборку", "11:20", "К", 5),
            new ChatItem("Дмитрий Орлов", "Спасибо за помощь!", "Вчера", "Д", 0),
            new ChatItem("Новости технологий", "Вышел новый релиз фреймворка", "Вчера", "Н", 0),
            new ChatItem("Мария Кузнецова", "Отправила файлы в личку", "Пн", "М", 0),
            new ChatItem("Ольга Петрова", "Ок, договорились", "Пн", "О", 0),
            new ChatItem("Рабочий чат", "Планёрка переносится на 15:00", "Вс", "Р", 0)
        };

        ChatsList.ItemsSource = _chats;
    }

    private void CreateChatButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new CreateChatPage());
    }

    private void ChatsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ChatItem? selected = ChatsList.SelectedItem as ChatItem;

        if (selected is null)
        {
            return;
        }

        ChatsList.SelectedItem = null;
        NavigationService?.Navigate(new ChatPage(selected));
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SearchPlaceholder is null)
        {
            return;
        }

        SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
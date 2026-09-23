using System.Windows;
using System.Windows.Controls;
using FakeMessenger.Core;

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
        ContactsListBox.ItemsSource = App.AppService.Contacts;
    }

    private void ChatType_Changed(object sender, RoutedEventArgs e)
    {
        if (GroupFieldsPanel is null)
        {
            return;
        }

        GroupFieldsPanel.Visibility = GroupChatRadio.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;

        // В режиме личного чата выбираем только один контакт
        ContactsListBox.SelectionMode = GroupChatRadio.IsChecked == true
            ? SelectionMode.Multiple
            : SelectionMode.Single;
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        if (PersonalChatRadio.IsChecked == true)
        {
            CreatePersonalChat();
        }
        else
        {
            CreateGroupChat();
        }
    }

    private void CreatePersonalChat()
    {
        if (ContactsListBox.SelectedItem is not User contact)
        {
            MessageBox.Show("Выберите контакт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        App.AppService.CreatePersonalChat(contact);
        NavigationService.GoBack();
    }

    private void CreateGroupChat()
    {
        if (ContactsListBox.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы одного участника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string chatName = ChatNameTextBox.Text.Trim();
        string groupName = GroupNameTextBox.Text.Trim();

        List<User> members = new();
        foreach (object item in ContactsListBox.SelectedItems)
        {
            if (item is User user)
            {
                members.Add(user);
            }
        }

        try
        {
            App.AppService.CreateGroup(chatName, groupName, members);
            NavigationService.GoBack();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
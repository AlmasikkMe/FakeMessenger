using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FakeMessenger.Core.Entities;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class CreateChatPage : ContentPage
{
    private IAppService? _appService;
    private NavigationService? _navigationService;

    public CreateChatPage()
    {
        InitializeComponent();
        Loaded += CreateChatPage_Loaded;
        
        _navigationService = (Application.Current as App)?.NavigationService;
        _appService = (Application.Current as App)?.AppService;
    }

    private void CreateChatPage_Loaded(object? sender, RoutedEventArgs e)
    {
        ContactsListBox.ItemsSource = _appService?.Contacts;
    }

    private void ChatType_Changed(object sender, RoutedEventArgs e)
    {
        if (GroupFieldsPanel is null)
        {
            return;
        }

        GroupFieldsPanel.IsVisible = GroupChatRadio.IsChecked ?? false;

        // В режиме личного чата выбираем только один контакт
        ContactsListBox.SelectionMode = GroupChatRadio.IsChecked == true
            ? SelectionMode.Toggle
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
            // MessageBox.Show("Выберите контакт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            _appService?.CreatePersonalChat(contact);
            _navigationService?.GoBack();
        }
        catch (Exception ex)
        {            
            // MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateGroupChat()
    {   
        string chatName = ChatNameTextBox.Text?.Trim() ?? string.Empty;
        string groupName = GroupNameTextBox.Text?.Trim() ?? string.Empty;

        List<User> members = [.. ContactsListBox.SelectedItems?.OfType<User>() ?? Array.Empty<User>()];

        try
        {
            _appService?.CreateGroup(chatName, groupName, members);
            _navigationService?.GoBack();
        }
        catch (Exception ex)
        {
            // MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _navigationService?.GoBack();
    }
}
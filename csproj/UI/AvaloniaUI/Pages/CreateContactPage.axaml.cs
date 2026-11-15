using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class CreateContactPage : ContentPage
{
    private IAppService? _appService;
    private NavigationService? _navigationService;

    public CreateContactPage()
    {
        InitializeComponent();
        _navigationService = (Application.Current as App)?.NavigationService;
        _appService = (Application.Current as App)?.AppService;
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text?.Trim() ?? string.Empty;
        string firstName = FirstNameTextBox.Text?.Trim() ?? string.Empty;
        string lastName = LastNameTextBox.Text?.Trim() ?? string.Empty;

        try
        {
            _appService?.CreateContact(username, firstName, lastName);
            _navigationService?.GoBack();
        }
        catch (System.Exception ex)
        {
            // MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _navigationService?.GoBack();
    }
}
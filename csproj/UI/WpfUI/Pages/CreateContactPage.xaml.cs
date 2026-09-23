using System.Windows;
using System.Windows.Controls;

namespace FakeMessenger.UI.WpfUI.Pages;

public partial class CreateContactPage : Page
{
    public CreateContactPage()
    {
        InitializeComponent();
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text.Trim();
        string firstName = FirstNameTextBox.Text.Trim();
        string lastName = LastNameTextBox.Text.Trim();

        try
        {
            App.AppService.CreateContact(username, firstName, lastName);
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
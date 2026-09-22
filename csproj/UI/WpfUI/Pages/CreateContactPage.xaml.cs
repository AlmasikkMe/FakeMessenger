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

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(firstName))
        {
            MessageBox.Show("Имя пользователя и имя обязательны для заполнения.",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

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
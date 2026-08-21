using Accessibility;
using System.Windows;

namespace FakeMessenger.UI.WpfUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainFrame.Navigate(new MainPage());
    }
}

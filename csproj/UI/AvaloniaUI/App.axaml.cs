using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FakeMessenger.Services;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class App : Application
{
    public MainWindow? MainWindow { get; set; }
    public IAppService? AppService { get; private set; }
    public INavigation? Navigation { get; private set; }

    public App()
    {
        AppService = null;
        Navigation = null;
    }

    public App(IAppService appService, INavigation navigationService)
    {
        AppService = appService;
        Navigation = navigationService;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow window = new(Navigation);
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
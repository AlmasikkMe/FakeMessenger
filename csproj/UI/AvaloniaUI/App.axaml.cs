using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FakeMessenger.Services;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public partial class App : Application
{
    public MainWindow? MainWindow { get; set; }
    public IAppService? AppService { get; private set; }
    public NavigationService? NavigationService { get; private set; }

    public App()
    {
        AppService = null;
        NavigationService = null;
    }

    public App(IAppService appService, NavigationService navigationService)
    {
        AppService = appService;
        NavigationService = navigationService;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow window = new();
            desktop.MainWindow = window;
            NavigationService.Navigation += window.NavigateTo; 
        }

        base.OnFrameworkInitializationCompleted();
    }
}
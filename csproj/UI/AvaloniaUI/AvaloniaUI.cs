using Avalonia;
using FakeMessenger.Services;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public class AvaloniaUI : IUserInterface
{
    private IAppService AppService;     
    private NavigationService NavigationService;

    public AvaloniaUI(Messenger messenger)
    {
        AppService = new MessengerService(messenger);
        NavigationService = new();
    }

    public void Run()
    {
        AppBuilder appBuilder = AppBuilder.Configure<App>(() => new App(AppService, NavigationService))
                                   .UsePlatformDetect()
#if DEBUG
                                   .WithDeveloperTools()
#endif
                                   .WithInterFont()
                                   .LogToTrace();

        appBuilder.StartWithClassicDesktopLifetime([]);
    }
}

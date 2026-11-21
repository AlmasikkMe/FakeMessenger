using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using FakeMessenger.Services;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public class AvaloniaUI : IUserInterface
{
    IAppService _appService;
    INavigation _navigation;

    public AvaloniaUI(Messenger messenger)
    {
        _appService = new MessengerService(messenger);
        _navigation = new NavigationPage()
        {
            PageTransition = new PageSlide(TimeSpan.FromMilliseconds(100), PageSlide.SlideAxis.Horizontal)
        };
    }

    public void Run()
    {
        AppBuilder appBuilder = AppBuilder
                                   .Configure(() => new App(_appService, _navigation))
                                   .UsePlatformDetect()
#if DEBUG
                                   .WithDeveloperTools()
#endif
                                   .WithInterFont()
                                   .LogToTrace();

        appBuilder.StartWithClassicDesktopLifetime([]);
    }
}

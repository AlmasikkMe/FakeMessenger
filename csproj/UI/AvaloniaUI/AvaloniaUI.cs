using Avalonia;
using FakeMessenger.Services;
using FakeMessenger.UI.AvaloniaUI.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public class AvaloniaUI : IUserInterface
{
    private Messenger _messenger;     
    public static IAppService AppService { get; set; }

    public AvaloniaUI(Messenger messenger)
    {
        AppService = new MessengerService(messenger);
        _messenger = messenger;
    }

    public void Run() => Run(isJoin: true);

    public void Run(bool isJoin)
    {
        AppBuilder app = AppBuilder.Configure<App>()
                                   .UsePlatformDetect()
#if DEBUG
                                   .WithDeveloperTools()
#endif
                                   .WithInterFont()
                                   .LogToTrace();

        app.StartWithClassicDesktopLifetime([]);
    }
}

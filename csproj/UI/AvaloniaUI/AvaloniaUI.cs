using Avalonia;
using FakeMessenger.Services;

namespace FakeMessenger.UI.AvaloniaUI;

public class AvaloniaUI : IUserInterface
{
    private Messenger _messenger;

    public AvaloniaUI(Messenger messenger)
    {
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

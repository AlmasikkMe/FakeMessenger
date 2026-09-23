using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI;

public class WpfUI : IUserInterface
{
    Messenger _messenger;
    
    public WpfUI(Messenger messenger)
    {
        _messenger = messenger;
    }

    public void Run() => Run(isJoin: true);

    public void Run(bool isJoin)
    {
        Thread thread = new(() =>
        {
            MainWindow mainWindow = new();
            App app = new(_messenger);
            app.Run(mainWindow);
        });
        
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (isJoin) thread.Join();
    }
}
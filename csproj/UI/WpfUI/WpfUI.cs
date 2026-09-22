using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI;

public class WpfUI : IUserInterface
{
    Messenger _messenger;
    
    public WpfUI(Messenger messenger)
    {
        _messenger = messenger;
    }

    public void Run()
    {
        Thread thread = new(() =>
        {
            MainWindow mainWindow = new();
            App app = new(_messenger);
            app.Run(mainWindow);
        });
        
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }
}
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI;

public class WpfUI : IUserInterface
{
    private Messenger _messenger;

    public WpfUI(Messenger messenger)
    {
        _messenger = messenger;
    }

    public void Run()
    {
        Thread thread = new(() =>
        {
            try
            {
                App app = new();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }
}
using FakeMessenger.Core;
using System.Windows;

namespace FakeMessenger.UI.WpfUI
{
    public class WpfUI : IUserInterface
    {
        MainWindow? _window;
        Messenger _messenger;

        public WpfUI(Messenger messenger)
        {
            _window = null;
            _messenger = messenger;
        }

        public void Run()
        {
            Thread wpfThread = new(() =>
            {
                Application app = new();
                app.Run( new MainWindow());
            });

            wpfThread.SetApartmentState(ApartmentState.STA);

            wpfThread.Start();
            wpfThread.Join();

        }
    }
}

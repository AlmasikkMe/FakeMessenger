using FakeMessenger.Core;
using System.Windows;

namespace FakeMessenger.UI.WpfUI;

public partial class WpfUI : Window, IUserInterface
{
    private Messenger _messenger;

    public WpfUI(Messenger messenger)
    {
        InitializeComponent();

        _messenger = messenger;
    }

    public void Run()
    {
        Thread wpfThread = new(() =>
        {
            var app = new Application();
            app.Run(this);
        });

        wpfThread.SetApartmentState(ApartmentState.STA);

        wpfThread.Start();
        wpfThread.Join();
    }
}

using System.Windows;
using FakeMessenger.UI.WpfUI.Services;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI;

public partial class App : Application
{
    public static IAppService AppService { get; private set; } = null!;

    public App(Messenger messenger)
    {
        AppService = new MessengerService(messenger);
    }
}
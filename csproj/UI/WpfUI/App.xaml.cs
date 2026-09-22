using System.Windows;
using FakeMessenger.UI.WpfUI.Services;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI;

public partial class App : Application
{
    public static IAppService AppService { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // В реальном приложении здесь вы должны создать экземпляр вашего сервиса,
        // который реализует IAppService и оборачивает Messenger.
        // Например:
        // AppService = new MessengerService(new Messenger(new Repository(...)));
        //
        // Для демонстрации используется заглушка, но вы должны заменить её.
        AppService = new DesignTimeAppService();
    }
}

/// <summary>
/// Временная заглушка только для дизайнера / демонстрации.
/// Удалите её и используйте реальный сервис.
/// </summary>
internal sealed class DesignTimeAppService : IAppService
{
    public User CurrentUser { get; } = new("@me", "Я");
    public IReadOnlyList<Chat> Chats { get; } = new List<Chat>();
    public IReadOnlyList<User> Contacts { get; } = new List<User>();

    public void CreateContact(string username, string firstName, string lastName) { }
    public void CreateGroup(string chatName, string groupName, List<User> members) { }
    public void CreatePersonalChat(User contact) { }
    public void SendMessage(Chat chat, string text, string type = "text") { }
    public void RemoveChat(Chat chat) { }
    public void RemoveContact(User contact) { }
    public void Save() { }
    public void Load() { }
}
using System.Collections.ObjectModel;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Services;

/// <summary>
/// Абстракция над ядром мессенджера.
/// Реализуйте этот интерфейс, обернув ваш класс Messenger.
/// </summary>
public interface IAppService
{
    User CurrentUser { get; }
    ObservableCollection<Chat> Chats { get; }
    ObservableCollection<User> Contacts { get; }

    void CreateContact(string username, string firstName, string lastName);
    void CreateGroup(string chatName, string groupName, List<User> members);
    void CreatePersonalChat(User contact);
    void SendMessage(Chat chat, string text, string type = "text");
    void RemoveChat(Chat chat);
    void RemoveContact(User contact);
    void Save();
    void Load();
}
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Services;

internal sealed class MessengerService : IAppService
{
    public User CurrentUser => _messenger.User;
    public IReadOnlyList<Chat> Chats => _messenger.Chats;
    public IReadOnlyList<User> Contacts => _messenger.Contacts;

    private Messenger _messenger;

    internal MessengerService(Messenger messenger)
    {
        _messenger = messenger;
    }

    public void CreateContact(string username, string firstName, string lastName)
    {
        _messenger.NewContact(username, firstName, lastName);
    }
    public void CreateGroup(string chatName, string groupName, List<User> members)
    {
        _messenger.NewGroup(chatName, groupName, members);
    }
    public void CreatePersonalChat(User contact)
    {
        Chat chat = new(contact.Username, contact.FullName);
        chat.AddMembers([CurrentUser, contact]);
        _messenger.AddChat(chat);
    }
    public void SendMessage(Chat chat, string text, string type = "text")
    {
        chat.AddMessage(CurrentUser, text, type, DateTime.Now);
    }
    public void RemoveChat(Chat chat)
    {
        _messenger.RemoveChat(chat);
    }
    public void RemoveContact(User contact)
    {
        _messenger.RemoveContact(contact);
    }
    public void Save()
    {
        _messenger.Save();
    }
    public void Load()
    {
        _messenger.Load();
    }
}
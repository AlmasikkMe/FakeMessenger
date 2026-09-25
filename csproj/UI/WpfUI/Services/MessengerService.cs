using System.Collections.ObjectModel;
using FakeMessenger.Core;

namespace FakeMessenger.UI.WpfUI.Services;

internal sealed class MessengerService : IAppService
{
    public User CurrentUser => _messenger.User;
    public ObservableCollection<Chat> Chats { get; private set; }
    public ObservableCollection<User> Contacts { get; private set; }

    private Messenger _messenger;

    internal MessengerService(Messenger messenger)
    {
        _messenger = messenger;
        UpdateData();
    }

    private void UpdateData()
    {
        Chats = new ObservableCollection<Chat>(_messenger.Chats);
        Contacts = new ObservableCollection<User>(_messenger.Contacts);
    }

    public void CreateContact(string username, string firstName, string lastName)
    {
        _messenger.NewContact(username, firstName, lastName);
        UpdateData();
    }
    public void CreateGroup(string chatName, string groupName, List<User> members)
    {
        _messenger.NewGroup(chatName, groupName, members);
        UpdateData();
    }
    public void CreatePersonalChat(User contact)
    {
        Chat chat = new(contact.Username, contact.FullName);
        chat.AddMembers([CurrentUser, contact]);
        _messenger.AddChat(chat);
        UpdateData();
    }
    public void SendMessage(User sender, Chat chat, string text, string type = "text")
    {
        chat.AddMessage(sender, text, type, DateTime.Now);
    }
    public void RemoveChat(Chat chat)
    {
        _messenger.RemoveChat(chat);
        UpdateData();
    }
    public void RemoveContact(User contact)
    {
        _messenger.RemoveContact(contact);
        UpdateData();
    }
    public void Save()
    {
        _messenger.Save();
    }
    public void Load()
    {
        _messenger.Load();
        UpdateData();
    }
}
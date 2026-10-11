using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FakeMessenger.Services;
using FakeMessenger.Core.Entities;
using System.Windows;


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

        Chats = new(messenger.ChatsRepository.Get());
        messenger.ChatsRepository.CollectionChanged += (s, e) => 
        {
            Application.Current.Dispatcher.Invoke(() => UpdateObservableCollection(messenger.ChatsRepository.Get(), Chats, e));
        };

        Contacts = new(messenger.ContactsRopository.Get());
        messenger.ContactsRopository.CollectionChanged += (s, e) => 
        {
            Application.Current.Dispatcher.Invoke(() => UpdateObservableCollection(messenger.ContactsRopository.Get(), Contacts, e));
        };
    }

    public static void UpdateObservableCollection<T>(IReadOnlyList<T> source,
                                                     ObservableCollection<T> collection, 
                                                     NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is null) break;
                foreach (T item in e.NewItems) collection.Add(item);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is null) break;
                foreach (T item in e.OldItems) collection.Remove(item);
                break;

            case NotifyCollectionChangedAction.Reset:
                collection.Clear();
                foreach (T i in source) collection.Add(i);
                break;
        }
    }

    public void CreateContact(string username, string firstName, string lastName)
    {
        _messenger.ContactsRopository.Add(new(username, firstName, lastName));
    }
    public void CreateGroup(string chatName, string groupName, List<User> members)
    {
        Chat group = new(chatName, groupName);
        foreach (User member in members) group.MembersRepository.Add(member);

        _messenger.ChatsRepository.Add(group);
    }
    public void CreatePersonalChat(User contact)
    {
        Chat chat = new(contact.Username, contact.FullName);
        chat.MembersRepository.Add(CurrentUser);
        chat.MembersRepository.Add(contact);
        _messenger.ChatsRepository.Add(chat);
    }
    public void SendMessage(User sender, Chat chat, string text, string type = "text", DateTime? dateTime = null)
    {
        Message message = new(sender) {
            Text = text,
            Type = type,
            DateTime = dateTime ?? DateTime.Now
        };

        chat.MessagesRepository.Add(message);
    }
    public void RemoveChat(Chat chat)
    {
        _messenger.ChatsRepository.Remove(chat);
    }
    public void RemoveContact(User contact)
    {
        _messenger.ContactsRopository.Remove(contact);
    }
    public void Save()
    {
        _messenger.FileRepository.Save(_messenger);
    }
    public void Load()
    {
        _messenger.FileRepository.Load(ref _messenger);
    }
}
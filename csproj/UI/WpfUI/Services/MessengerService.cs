using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        Chats = new(messenger.ChatsRepository.Get());
        messenger.ChatsRepository.CollectionChanged += (s, e) => 
        {
            UpdateObservableCollection(Chats, e, () => Chats = new(messenger.ChatsRepository.Get()));
        };

        Contacts = new(messenger.ContactsRopository.Get());
        messenger.ContactsRopository.CollectionChanged += (s, e) => 
        {
            UpdateObservableCollection(Contacts, e, () => Contacts = new(messenger.ContactsRopository.Get()));
        };
    }

    private void UpdateObservableCollection<T>(ObservableCollection<T> collection, 
                                               NotifyCollectionChangedEventArgs eventArgs,
                                               Action reset)
    {
        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (eventArgs.NewItems is null) break;
                foreach (T item in eventArgs.NewItems)
                {
                    collection.Add(item);
                };
                break;

            case NotifyCollectionChangedAction.Remove:
                if (eventArgs.OldItems is null) break;
                foreach (T item in eventArgs.OldItems)
                {
                    collection.Remove(item);
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                reset();
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
        group.AddMembers(members);

        _messenger.ChatsRepository.Add(group);
    }
    public void CreatePersonalChat(User contact)
    {
        Chat chat = new(contact.Username, contact.FullName);
        chat.AddMembers([CurrentUser, contact]);
        _messenger.ChatsRepository.Add(chat);
    }
    public void SendMessage(User sender, Chat chat, string text, string type = "text", DateTime? dateTime = null)
    {
        chat.AddMessage(sender, text, type, dateTime);
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
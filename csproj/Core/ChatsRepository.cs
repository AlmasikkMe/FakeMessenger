using System.Collections.Specialized;

namespace FakeMessenger.Core;

public class ChatsRepository : ICollectionRepository<Chat>
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged = null;
    private List<Chat> _chats = [];

    public IReadOnlyList<Chat> Get()
    {
        return _chats.AsReadOnly();
    }

    public void Add(Chat chat)
    {
        if (_chats.Any(c => c.ChatName == chat.ChatName)) 
            throw new ArgumentException($"Чат с chatname {chat.ChatName} уже существует в коллекции.");
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Add, 
            chat,
            _chats.Count
        );

        _chats.Add(chat);

        CollectionChanged?.Invoke(this, eventArgs);
    }
    
    public void Remove(Chat chat)
    {
        if (!_chats.Contains(chat)) 
            throw new InvalidOperationException($"Указанный экземпляр чата {chat.ChatName} не содержится в коллекции.");
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Remove, 
            chat,
            _chats.IndexOf(chat)
        );

        _chats.Remove(chat);

        CollectionChanged?.Invoke(this, eventArgs);
    }

    public void Clear()
    {
        _chats.Clear();

        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Reset
        );

        CollectionChanged?.Invoke(this, eventArgs);
    }
}

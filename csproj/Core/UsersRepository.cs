using System.Collections.Specialized;

namespace FakeMessenger.Core;

public class UsersRepository : ICollectionRepository<User>
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged = null;
    private List<User> _users = [];

    public IReadOnlyList<User> Get()
    {
        return _users.AsReadOnly();
    }

    public void Add(User user)
    {
        if (_users.Any(u => u.Username == user.Username)) 
            throw new ArgumentException($"Пользователь с username {user.Username} уже существует в коллекции.");
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Add, 
            user,
            _users.Count
        );

        _users.Add(user);

        CollectionChanged?.Invoke(this, eventArgs);
    }
    
    public void Remove(User user)
    {
        if (!_users.Contains(user)) 
            throw new InvalidOperationException($"Указанный экземпляр пользователя {user.Username} не содержится в коллекции.");
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Remove, 
            user,
            _users.IndexOf(user)
        );

        _users.Remove(user);

        CollectionChanged?.Invoke(this, eventArgs);
    }

    public void Load(ICollectionRepository<User> repository)
    {
        _users = repository.Get().ToList();

        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Reset
        );

        CollectionChanged?.Invoke(this, eventArgs);
    }
}
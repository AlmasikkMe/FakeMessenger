using System.Collections.Specialized;

namespace FakeMessenger.Core.Repositories;

public abstract class BaseCollectionRepository<T> : ICollectionRepository<T>
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged = null;
    protected readonly List<T> _items = [];

    protected abstract void ThrowIfInvalid(T item);

    public virtual IReadOnlyList<T> Get()
    {
        return _items.AsReadOnly();
    }

    public virtual void Add(T item)
    {
        ThrowIfInvalid(item);

        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Add, 
            item,
            _items.Count
        );

        _items.Add(item);

        CollectionChangedInvoke(eventArgs);
    }
    
    public void Remove(T chat)
    {   
        int index = _items.IndexOf(chat);
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Remove, 
            chat,
            index
        );

        _items.RemoveAt(index);

        CollectionChangedInvoke(eventArgs);
    }

    public void Clear()
    {
        _items.Clear();

        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Reset
        );

        CollectionChangedInvoke(eventArgs);
    }

    protected void CollectionChangedInvoke(NotifyCollectionChangedEventArgs eventArgs)
    {
        CollectionChanged?.Invoke(this, eventArgs);
    }
}
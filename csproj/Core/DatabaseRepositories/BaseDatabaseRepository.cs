using System.Collections.Specialized;

namespace FakeMessenger.Core.DatabaseRepositories;

public abstract class BaseDatabaseRepository<T> : IDatabaseRepository<T>
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public BaseDatabaseRepository()
    {
        CollectionChanged = null;
        InitDatabase();
    }

    protected abstract void InitDatabase();
    public abstract T? GetValue(string key);
    public abstract IEnumerable<string> EnumerateKeys();
    public abstract void Remove(T item);

    protected abstract void AddValue(T value);
    protected abstract void RemoveValue(string key);
    protected abstract void ClearValues();

    public virtual void Add(T item)
    {
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Add, 
            item,
            EnumerateKeys().ToList().Count
        );

        AddValue(item);

        CollectionChangedInvoke(eventArgs);
    }

    protected void Remove(T item, string key)
    {   
        int index = EnumerateKeys().ToList().IndexOf(key);
        
        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Remove, 
            item,
            index
        );

        RemoveValue(key);

        CollectionChangedInvoke(eventArgs);
    }

    public void Clear()
    {
        ClearValues();

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
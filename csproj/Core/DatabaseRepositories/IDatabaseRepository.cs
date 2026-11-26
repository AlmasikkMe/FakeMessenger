using System.Collections.Specialized;

namespace FakeMessenger.Core.DatabaseRepositories;

public interface IDatabaseRepository<T>
{
    event NotifyCollectionChangedEventHandler CollectionChanged;
    
    T? GetValue(string key);
    IEnumerable<string> EnumerateKeys();

    void Add(T element);
    void Remove(T element);
    void Clear();
}
using System.Collections.Specialized;

namespace FakeMessenger.Core;

public interface ICollectionRepository<T>
{
    event NotifyCollectionChangedEventHandler CollectionChanged;
    IReadOnlyList<T> Get();
    void Add(T element);
    void Remove(T element);
    void Load(ICollectionRepository<T> repository);
}
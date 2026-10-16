using System.Collections.Specialized;
using FakeMessenger.Core.Entities;

namespace FakeMessenger.Core.Repositories;

public class MessagesRepository : BaseCollectionRepository<Message>
{
    protected override void ThrowIfInvalid(Message message) { }
    
    public override void Add(Message item)
    {
        ThrowIfInvalid(item);

        int index = 0;
        
        for (int i = _items.Count - 1; i > 0; i--)
        {
            if (_items[i].DateTime <= item.DateTime)
            {
                index = i + 1;
                break;
            }
        }

        _items.Insert(index, item);

        NotifyCollectionChangedEventArgs eventArgs = new(
            NotifyCollectionChangedAction.Add, 
            item,
            index
        );

        CollectionChangedInvoke(eventArgs);
    }
}
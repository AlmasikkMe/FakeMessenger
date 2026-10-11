using System.Collections.Specialized;
using FakeMessenger.Core.Entities;

namespace FakeMessenger.Core.Repositories;

public class ChatsRepository : BaseCollectionRepository<Chat>
{
    protected override void ThrowIfInvalid(Chat chat)
    {
        if (_items.Any(c => c.ChatName == chat.ChatName)) 
            throw new ArgumentException($"Чат с chatname {chat.ChatName} уже существует в коллекции.");
    }
}

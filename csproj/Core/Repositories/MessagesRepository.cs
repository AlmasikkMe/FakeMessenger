using FakeMessenger.Core.Entities;

namespace FakeMessenger.Core.Repositories;

public class MessagesRepository : BaseCollectionRepository<Message>
{
    protected override void ThrowIfInvalid(Message message) { }
}
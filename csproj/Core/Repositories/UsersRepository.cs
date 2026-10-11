using FakeMessenger.Core.Entities;

namespace FakeMessenger.Core.Repositories;

public class UsersRepository : BaseCollectionRepository<User>
{
    protected override void ThrowIfInvalid(User user)
    {
        if (_items.Any(u => u.Username == user.Username)) 
            throw new ArgumentException($"Пользователь с username {user.Username} уже существует в коллекции.");
    }
}
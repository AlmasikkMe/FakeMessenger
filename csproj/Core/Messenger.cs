using FakeMessenger.FileRepository;

namespace FakeMessenger.Core;

public class Messenger(FileRepository.FileRepository fileRepos,
                       ICollectionRepository<User> contactsRepos,
                       ICollectionRepository<Chat> chatRepos,
                       User? user = null)
{
    public User User { get; } = user ?? new("@FakeChat", "Вы");
    public ICollectionRepository<User> ContactsRopository { get; } = contactsRepos;
    public ICollectionRepository<Chat> ChatsRepository { get; } = chatRepos;
    public FileRepository.FileRepository FileRepository { get; } = fileRepos;
}

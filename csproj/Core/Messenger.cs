using FakeMessenger.FileRepository;

namespace FakeMessenger.Core;

public class Messenger(FileRepository.FileRepository fileRepository, User? user = null)
{
    private FileRepository.FileRepository _fileRepository = fileRepository;
    public User User => _user;
    private User _user = user ?? new("@FakeChat", "Вы");
    public ICollectionRepository<User> ContactsRopository { get; private set; }
    public IReadOnlyList<Chat> Chats => _chats.AsReadOnly();
    private List<Chat> _chats = [];

    public void NewGroup(string chatName, string groupName, List<User> members)
    {
        if (!members.Contains(User))
            members = members.Prepend(User).ToList();

        if (chatName.IsWhiteSpace())
            chatName = $"@chat{_chats.Count + 1}";

        if (chatName[0] is not '@')
            chatName = $"@{chatName}";

        if (_chats.Any(chat => chat.ChatName == chatName))
            throw new ArgumentException("Чат с таким уникальным именем уже существует!");

        if (members.Count == 1)
            throw new ArgumentException("Требуется как минимум 1 участник группы");

        if (members.Union(ContactsRopository.Get()).GroupBy(member => member.Username).Any(g => g.Count() > 1))
            throw new ArgumentException("Обнаружены разные объекты с одинаковым UserName!");

        if (groupName.IsWhiteSpace())
        {
            groupName = string.Join(", ", members.Take(3).Select(member => member.FullName));

            if (members.Count > 3) groupName += $" и ещё {members.Count - 3}";
        }


        Chat chat = new(chatName, groupName);
        chat.AddMembers(members);

        _chats.Add(chat);
    }

    [Obsolete("Используйте свойство Chats для получения всех чатов.")]
    public List<Chat> GetChats() => Chats.ToList();
    public List<Chat> GetChats(string search)
    {
        return (from chat in _chats
                where chat.ChatName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select chat)
                .ToList();
    }
    public void AddChat(Chat chat)
    {
        if (_chats.Select(chat => chat.ChatName).Contains(chat.ChatName))
            throw new ArgumentException($"Чат {chat.ChatName} уже существует");

        _chats.Add(chat);
    }
    public void RemoveChat(Chat chat)
    {
        if (!_chats.Remove(chat)) throw new ArgumentException($"Чат {chat.ChatName} не найден");
    }

    public void Save()
    {
        _fileRepository.Save(this);
    }

    public void Load()
    {
        Messenger messenger = _fileRepository.Load();

        _user = messenger.User;
        this.ContactsRopository.Load(messenger.ContactsRopository);
        _chats = messenger.Chats.ToList();
    }
}

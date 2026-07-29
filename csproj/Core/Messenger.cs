using FakeMessenger.FileRepository;

namespace FakeMessenger.Core;

public class Messenger(Repository fileRepository, User? user = null)
{
    private Repository _fileRepository = fileRepository;
    public User User => _user;
    private User _user = user ?? new("@FakeChat", "Вы");
    public IReadOnlyList<User> Contacts => _contacts.AsReadOnly();
    private List<User> _contacts = [];
    public IReadOnlyList<Chat> Chats => _chats.AsReadOnly();
    private List<Chat> _chats = [];

    public void NewContact(string username, string firstName, string lastName = "")
    {
        if (username.IsWhiteSpace()) username = $"@user{_contacts.Count + 1}";
        if (firstName.IsWhiteSpace()) firstName = $"Контакт {_contacts.Count + 1}";

        if (!username.StartsWith("@")) username = $"@{username}";

        if (_contacts.Any(contact => contact.Username == username))
            throw new ArgumentException($"Пользователь с именем {username} уже существует!");

        _contacts.Add(new(username, firstName, lastName));
    }

    public void NewContact(User user)
    {
        if (!user.Username.StartsWith("@")) user.Username = $"@{user.Username}";
        if (_contacts.Any(contact => contact.Username == user.Username))
            throw new ArgumentException($"Пользователь с именем {user.Username} уже существует!");

        _contacts.Add(user);
    }

    public void NewGroup(string chatName, string groupName, List<User> members)
    {
        if (!members.Contains(User))
            members = members.Prepend(User).ToList();

        if (chatName.IsWhiteSpace())
            chatName = $"@chat{_chats.Count + 1}";

        if (_chats.Any(chat => chat.ChatName == chatName))
            throw new ArgumentException("Чат с таким уникальным именем уже существует!");

        if (members.Count == 1)
            throw new ArgumentException("Требуется как минимум 1 участник группы");

        if (members.Union(_contacts).GroupBy(member => member.Username).Any(g => g.Count() > 1))
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
    public List<Chat> GetChats(string search = "")
    {
        return (from chat in _chats
                where chat.ChatName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select chat)
                .ToList();
    }
    public List<User> GetContacts(string search = "")
    {
        return (from contact in _contacts
                where contact.Username.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select contact)
                .ToList();
    }
    public void AddChat(Chat chat)
    {
        if (_chats.Select(chat => chat.ChatName).Contains(chat.ChatName))
            throw new ArgumentException($"Чат {chat.ChatName} уже существует");

        _chats.Add(chat);
    }

    public void Save()
    {
        _fileRepository.Save(this);
    }

    public void Load()
    {
        Messenger messenger = _fileRepository.Load();

        _user = messenger.User;
        _contacts = messenger.Contacts.ToList();
        _chats = messenger.Chats.ToList();
    }
}

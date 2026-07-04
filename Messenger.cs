using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Xml.Serialization;

namespace ConsoleFakeChat;

[XmlRoot("Messager")]
public class XMLMessagerExport
{
    public User User { get; set; } = Messenger.User;

    [XmlArray("Contacts")]
    [XmlArrayItem("Contact")]
    public List<User> Contacts = [];

    [XmlArray("Chats")]
    [XmlArrayItem("Chat")]
    public List<Chat> Chats = [];
}

public class Messenger()
{
    public static User User => _user;
    private static User _user = new("@FakeChat", "Вы");
    public FileInfo SaveFile { get; set; } = new("Save.Messager.xml");
    public IReadOnlyList<User> Contacts => _contacts.AsReadOnly();
    private List<User> _contacts = [];
    public IReadOnlyList<Chat> Chats => _chats.AsReadOnly();
    private List<Chat> _chats = [];

    public void NewContact(string username, string firstName, string lastName = "")
    {
        if (username.IsWhiteSpace()) username = $"@user{_contacts.Count + 1}";
        if (firstName.IsWhiteSpace()) firstName =  $"Контакт {_contacts.Count + 1}";

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

    public void NewGroup(string groupName, List<User> members)
    {
        if (!members.Contains(User)) members.Prepend(User);

        if (members.Count == 1) throw new ArgumentException("Требуется как минимум 1 участник группы");

        if (members.Union(_contacts).GroupBy(member => member.Username).Any(g => g.Count() > 1)) 
            throw new ArgumentException("Обнаружены разные объекты с одинаковым UserName!");

        if (groupName.IsWhiteSpace()) 
        {
            groupName = string.Join(", ", members.Take(3).Select(member => member.FullName));

            if (members.Count > 3) groupName += $" и ещё {members.Count - 3}";
        }

        Chat chat = new(groupName);
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
        if (_chats.Contains(chat)) 
            throw new ArgumentException($"Чат {chat.ChatName} уже существует");
        
        _chats.Add(chat);
    }

    public void Save()
    {
        XMLMessagerExport export = new()
        {
            Contacts = _contacts,
            Chats = _chats
        };

        XmlSerializer serializer = new(typeof(XMLMessagerExport));

        using (StreamWriter writer = new(SaveFile.FullName))
        {
            serializer.Serialize(writer, export);
        }
    }

    public void Load()
    {
        XmlSerializer serializer = new(typeof(XMLMessagerExport));
        XMLMessagerExport? serialized;

        using (FileStream fileStream = new FileStream(SaveFile.FullName, FileMode.OpenOrCreate))
        {
            serialized = (XMLMessagerExport?)(serializer.Deserialize(fileStream));
        }

        if (serialized == null)
        {
            string reason = SaveFile.Length == 0
                ? "пуст"
                : "содержит неверные данные";

            throw new InvalidOperationException($"Файл {SaveFile.Name} {reason}.");
        }

        _contacts = [];
        _chats = [];

        _user = serialized.User;
        serialized.Contacts.ForEach(NewContact);
        serialized.Chats.ForEach(AddChat);
    }
}

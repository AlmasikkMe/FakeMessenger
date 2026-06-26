using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ConsoleFakeChat;

public class Messenger()
{
    public User User { get; } = new User("@FakeChat", "Вы");

    [XmlArray("Contacts")]
    [XmlArrayItem("User")]
    public List<User> Contacts { get => field; private set; } = [];

    [XmlArray("Chats")]
    [XmlArrayItem("Chat")]
    public List<Chat> Chats { get => field; private set; } = [];

    public void NewContact(string username, string firstName, string lastName = "")
    {
        if (username.IsWhiteSpace()) username = $"@user{Contacts.Count + 1}";
        if (firstName.IsWhiteSpace()) firstName =  $"Контакт {Contacts.Count + 1}";

        if (!username.StartsWith("@")) username = $"@{username}";

        if (Contacts.Any(contact => contact.Username == username)) 
            throw new ArgumentException($"Пользователь с именем {username} уже существует!");

        Contacts.Add(new(username, firstName, lastName));
    }

    public void NewGroup(string groupName, List<User> members)
    {
        if (!members.Contains(User)) members.Prepend(User);

        if (members.Count == 1) throw new ArgumentException("Требуется как минимум 1 участник группы");

        if (members.Union(Contacts).GroupBy(member => member.Username).Any(g => g.Count() > 1)) 
            throw new ArgumentException("Обнаружены разные объекты с одинаковым UserName!");

        if (groupName.IsWhiteSpace()) 
        {
            groupName = string.Join(", ", members.Take(3).Select(member => member.FullName));

            if (members.Count > 3) groupName += $" и ещё {members.Count - 3}";
        }

        Chat chat = new(groupName);
        chat.AddMembers(members);

        Chats.Add(chat);
    }
    public List<Chat> GetChats(string search = "")
    {
        return (from chat in Chats
                where chat.ChatName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select chat)
                .ToList();
    }
    public List<User> GetContacts(string search = "")
    {
        return (from contact in Contacts
                where contact.Username.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select contact)
                .ToList();
    }
    
    public void Save()
    {
        XmlSerializer serializer = new(typeof(Messenger));

        using (StreamWriter writer = new("Save.Messager.xml"))
        {
            serializer.Serialize(writer, this);
        }
    }

    public void Load()
    {
        XmlSerializer serializer = new(typeof(Messenger));
        Messenger? serialized;

        using (FileStream fileStream = new FileStream("Save.Messager.xml", FileMode.OpenOrCreate))
        {
            serialized = (Messenger?)(serializer.Deserialize(fileStream));
        }

        if (serialized == null) return;

        Contacts = serialized.Contacts;
        Chats = serialized.Chats;
    }
}

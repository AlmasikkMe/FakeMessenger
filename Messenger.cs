using System.Runtime.Serialization;
using System.Xml.Serialization;

public class Messenger
{
    private List<string> _contacts = [];
    private List<Chat> _chats = [];

    [XmlArray("Contacts")]
    [XmlArrayItem("Contact")]
    public List<string> Contacts
    {
        get => _contacts;
        set => _contacts = value ?? [];
    }

    [XmlArray("Chats")]
    [XmlArrayItem("Chat")]
    public List<Chat> Chats
    {
        get => _chats;
        set => _chats = value ?? [];
    }

    public void NewContact(string contactName)
    {
        contactName = contactName.IsWhiteSpace() ? $"Контакт {_contacts.Count + 1}" : contactName.Trim();
        _contacts.Add(contactName);

        Chat chat = new(contactName);
        chat.AddMembers(["Вы", contactName]);

        _chats.Add(chat);
    }

    public void NewGroup(string groupName, string[] members)
    {
        if (members.Length == 0) return;

        List<string> membersList = ["Вы"];

        for (int i = 0; i < members.Length; i++)
        {
            if (!members[i].IsWhiteSpace()) membersList.Add(members[i].Trim());
        }

        if (groupName.IsWhiteSpace()) 
        {
            groupName = string.Join(", ", membersList.Take(3));

            if (membersList.Count > 3) groupName += $" и ещё {membersList.Count - 3}";
        }

        Chat chat = new(groupName);
        chat.AddMembers(membersList);

        _chats.Add(chat);
    }
    public List<Chat> GetChats(string search = "")
    {
        return (from chat in _chats
                where chat.ChatName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select chat)
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
}

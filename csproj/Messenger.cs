using System.Diagnostics;
using System.Xml.Linq;

namespace FakeMessenger;

public class Messenger()
{
    public static User User => _user;
    private static User _user = new("@FakeChat", "Вы");
    public List<FileInfo> OldSaveFiles { get; set; } = [new("Save.Messager.xml")];
    public FileInfo SaveFile { get; set; } = new("Messenger.Save.xml");
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
        XDocument doc = new(
            new XElement("Messenger",
                User.ToXElement(),
                new XElement("Contacts", from user in _contacts select user.ToXElement()),
                new XElement("Chats", from chat in _chats select chat.ToXElement())
                )
            );

        doc.Save(SaveFile.FullName);
    }

    public void Load(Action<string>? elementNullHandler = null, Action<string, Exception>? elementExceptionHandler = null)
    {
        if (!SaveFile.Exists)
        {
            if (OldSaveFiles.Any(file => file.Exists)) RenameOldFile();
            else throw new InvalidOperationException("Файлы сохранения не найдены");
        }

        XDocument doc = XDocument.Load(SaveFile.FullName);

        if (doc.Root is null) throw new InvalidOperationException($"Не удалось получить корневой элемент в {SaveFile.FullName}");

        XElement? userElement = doc.Root.Element("User");
        if (userElement is null) { if (elementNullHandler is not null) elementNullHandler("User"); }
        else _user = new(userElement);

        XElement? contactsElement = doc.Root.Element("Contacts");
        if (contactsElement is null) { if (elementNullHandler is not null) elementNullHandler("Contacts"); }
        else
        {
            List<User> contacts = _contacts;
            try
            {
                _contacts.Clear();
                contactsElement
                    .Elements("User")
                    .Select(user => new User(user))
                    .ToList()
                    .ForEach(NewContact);
            }
            catch (Exception ex) 
            { 
                _contacts = contacts;
                if (elementExceptionHandler is not null) elementExceptionHandler("Contacts", ex);
            }
        }

        XElement? chatsElement = doc.Root.Element("Chats");
        if (chatsElement is null) { if (elementNullHandler is not null) elementNullHandler("Chats"); }
        else
        {
            List<Chat> chats = _chats;
            try
            {
                _chats.Clear();
                chatsElement
                    .Elements("Chat")
                    .Select(chat => new Chat(chat, Contacts.Append(User).ToList()))
                    .ToList()
                    .ForEach(AddChat);
            }
            catch (Exception ex) 
            { 
                _chats = chats;
                if (elementExceptionHandler is not null) elementExceptionHandler("Chats", ex);
            }
        }
    }

    public void RenameOldFile()
    {
        if (SaveFile.Exists) throw new InvalidOperationException("Файл сохранения уже существует");

        foreach (var file in OldSaveFiles)
        {
            if (file.Exists)
            {
                File.Copy(file.FullName, SaveFile.FullName);
                File.Delete(file.FullName);
                return;
            }
        }

        throw new InvalidOperationException("Нет файлов для восстановления");
    }
}

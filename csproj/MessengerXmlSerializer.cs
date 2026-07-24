using System.Xml.Linq;

namespace FakeMessenger;
public static class MessengerXmlSerializer
{
    public static List<FileInfo> OldSaveFiles { get; set; } = [new("Save.Messager.xml")];
    public static FileInfo SaveFile { get; set; } = new("Messenger.Save.xml");
    public static void SerializeAndSave(Messenger messenger) => Serialize(messenger).Save(SaveFile.FullName);

    public static XDocument Serialize(Messenger messenger) => 
        new(new XElement("Messenger",
            messenger.User.ToXElement(),
            new XElement("Contacts", from user in messenger.Contacts select user.ToXElement()),
            new XElement("Chats", from chat in messenger.Chats select chat.ToXElement())
            ));

    private static InvalidOperationException DeserializeFailed(string element, string location) => new($"Не найден обязательный элемент {element} в {location}");

    public static Messenger LoadAndDeserializeMessenger() => DeserializeMessenger(XDocument.Load(SaveFile.FullName));

    public static Messenger DeserializeMessenger(XDocument doc)
    {
        User user;
        List<User> contacts;
        List<Chat> chats;

        if (doc.Root is null) throw new InvalidOperationException($"Не удалось получить корневой элемент в {SaveFile.FullName}");

        XElement userElement = doc.Root.Element("User") ?? throw DeserializeFailed("User", SaveFile.FullName);
        user = DeserializeUser(userElement);

        XElement contactsElement = doc.Root.Element("Contacts") ?? throw DeserializeFailed("Contacts", SaveFile.FullName);
        contacts = contactsElement.Elements("User")
                                    .Select(DeserializeUser)
                                    .ToList();

        XElement chatsElement = doc.Root.Element("Chats") ?? throw DeserializeFailed("Chats", SaveFile.FullName);
        chats = chatsElement.Elements("Chat")
                            .Select(chat => DeserializeChat(chat, contacts.Prepend(user).ToList()))
                            .ToList();

        Messenger messenger = new(user);

        contacts.ForEach(messenger.NewContact);
        chats.ForEach(messenger.AddChat);

        return new(user);
    }

    public static void RenameOldFile()
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

    public static User DeserializeUser(XElement xElement)
    {
        string username = xElement.Attribute("Username")?.Value ?? throw DeserializeFailed("Username", "User");
        string firstName = xElement.Attribute("FirstName")?.Value ?? throw DeserializeFailed("FirstName", $"User ({username})");
        string lastName = xElement.Attribute("LastName")?.Value ?? "";

        return new(username, firstName, lastName);
    }

    public static Chat DeserializeChat(XElement xElement, List<User> users) => DeserializeChat(xElement, users.AsReadOnly());
    public static Chat DeserializeChat(XElement xElement, IReadOnlyList<User> users)
    {
        string chatName = xElement.Attribute("ChatName")?.Value ?? throw DeserializeFailed("ChatName", "Chat");
        string name = xElement.Attribute("Name")?.Value ?? throw DeserializeFailed("Name", $"Chat ({chatName})");

        Chat chat = new(chatName, name);

        XElement membersElement = xElement.Element("Members") ?? throw DeserializeFailed("Members", $"Chat ({chatName})");
        List<string> membersUsernames = membersElement
            .Elements("User")
            .Select(member => member.Value)
            .ToList();

        chat.AddMembers(
            membersUsernames
            .Select(username =>
                users.FirstOrDefault(user => user.Username == username) ??
                    throw new InvalidOperationException($"Пользователь {username} из элемента Members {chatName} не найден")
                )
            .ToList());

        XElement? messagesElement = xElement.Element("Messages");
        messagesElement?.Elements("Message")
                        .ToList()
                        .ForEach(el => chat.AddMessage(DeserializeMessage(el, chat.Members)));

        return chat;
    }

    public static Message DeserializeMessage(XElement messageElement, List<User> chatMembers) => DeserializeMessage(messageElement, chatMembers.AsReadOnly());
    public static Message DeserializeMessage(XElement messageElement, IReadOnlyList<User> chatMembers)
    {
        string senderUsername = messageElement.Attribute("Sender")?.Value ?? throw DeserializeFailed("Sender", $"Message");
        User sender = chatMembers.FirstOrDefault(member => member.Username == senderUsername) ??
            throw new InvalidOperationException($"Отправитель {senderUsername} не числится в участниках чата");

        string? text = messageElement.Value.Trim();
        string type = messageElement.Attribute("Type")?.Value ?? throw DeserializeFailed("Type", "Message");

        string dateTimeValue = messageElement.Attribute("DateTime")?.Value ?? throw DeserializeFailed("DateTime", $"Message");
        if (dateTimeValue.IsWhiteSpace()) 
            throw new InvalidOperationException($"Пустой элемент DateTime в элементе Message");

        DateTime dateTime;
        if (!DateTime.TryParse(dateTimeValue, out dateTime)) throw new InvalidOperationException($"Неудачный парсинг элемента DateTime в элементе Message");

        return new(sender) { Text = text, Type = type, DateTime = dateTime };
    }
}

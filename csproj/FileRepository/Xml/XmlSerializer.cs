using FakeMessenger.Core;
using System.Xml.Linq;

namespace FakeMessenger.FileRepository.Xml;

public class XmlSerializer
{
    public XDocument SerializeMessenger(Messenger messenger) =>
        new(new XElement("_messenger",
            SerializeUser(messenger.User),
            new XElement("Contacts", from u in messenger.Contacts select SerializeUser(u)),
            new XElement("Chats", from c in messenger.Chats select SerializeChat(c))
            ));

    public XElement SerializeUser(User user) =>
        new("User",
            new XAttribute("Username", user.Username),
            new XAttribute("FirstName", user.FirstName),
            new XAttribute("LastName", user.LastName),
            new XAttribute("IsDeleted", user.IsDeleted)
            );

    public XElement SerializeChat(Chat chat) =>
        new("Chat",
            new XAttribute("ChatName", chat.ChatName),
            new XAttribute("Name", chat.Name),
            new XElement("Members", from m in chat.Members select new XElement("User", m.Username)),
            new XElement("Messages", from m in chat.Messages select SerializeMessage(m))
            );

    public XElement SerializeMessage(Message message) =>
        new("Message",
            new XAttribute("Sender", message.Sender.Username),
            new XAttribute("Type", message.Type),
            new XAttribute("DateTime", message.DateTime),
            message.Text
            );



    private InvalidOperationException DeserializeFailed(string element, IEnumerable<string> locations) =>
        new($"Не найден обязательный элемент {element} в {string.Join(" - ", locations)}");

    private string GetAttributeOrElementValue(XElement xElement, string name, IEnumerable<string> location) =>
        xElement.Attribute(name)?.Value ??
        xElement.Element(name)?.Value ??
        throw DeserializeFailed(name, location);

    public Messenger DeserializeMessenger(XDocument doc, Repository fileRepository, IEnumerable<string> locations)
    {
        User user;
        List<User> contacts;
        List<Chat> chats;

        if (doc.Root is null) throw new InvalidOperationException($"Не удалось получить корневой элемент в {locations}");

        XElement userElement = doc.Root.Element("User") ?? throw DeserializeFailed("User", locations);
        user = DeserializeUser(userElement, locations);

        XElement contactsElement = doc.Root.Element("Contacts") ?? throw DeserializeFailed("Contacts", locations);
        contacts = contactsElement.Elements("User")
                                    .Select(el => DeserializeUser(el, locations.Append("Contacts")))
                                    .ToList();

        XElement chatsElement = doc.Root.Element("Chats") ?? throw DeserializeFailed("Chats", locations);
        chats = chatsElement.Elements("Chat")
                            .Select(chat => DeserializeChat(chat, contacts.Prepend(user).ToList(), locations.Append("Chats")))
                            .ToList();

        Messenger messenger = new(fileRepository, user);

        contacts.ForEach(messenger.NewContact);
        chats.ForEach(messenger.AddChat);

        return messenger;
    }

    public User DeserializeUser(XElement xElement, IEnumerable<string> locations)
    {
        string username = GetAttributeOrElementValue(xElement, "Username", locations.Append("User"));

        string firstName = GetAttributeOrElementValue(xElement, "FirstName", locations.Append($"User ({username})"));

        string lastName;

        try { lastName = GetAttributeOrElementValue(xElement, "LastName", locations); } catch { lastName = ""; }

        bool isDeleted = (bool?)xElement.Attribute("IsDeleted") ?? false;

        return new(username, firstName, lastName);
    }

    public Chat DeserializeChat(XElement xElement, List<User> users, IEnumerable<string> locations) =>
        DeserializeChat(xElement, users.AsReadOnly(), locations);
    public Chat DeserializeChat(XElement xElement, IReadOnlyList<User> users, IEnumerable<string> locations)
    {
        string chatName = GetAttributeOrElementValue(xElement, "ChatName", locations.Append("Chat"));

        string name = GetAttributeOrElementValue(xElement, "Name", locations.Append($"Chat ({chatName})"));

        Chat chat = new(chatName, name);

        XElement membersElement = xElement.Element("Members") ?? throw DeserializeFailed("Members", locations.Append($"Chat ({chatName})"));

        List<string> membersUsernames = membersElement.Elements("User")
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
                        .ForEach(el => chat.AddMessage(DeserializeMessage(el, chat.Members, locations.Append("Messages"))));

        return chat;
    }

    public Message DeserializeMessage(XElement messageElement, List<User> chatMembers, IEnumerable<string> locations) => DeserializeMessage(messageElement, chatMembers.AsReadOnly(), locations);
    public Message DeserializeMessage(XElement messageElement, IReadOnlyList<User> chatMembers, IEnumerable<string> locations)
    {
        string senderUsername = GetAttributeOrElementValue(messageElement, "Sender", locations.Append($"Message"));
        User sender = chatMembers.FirstOrDefault(member => member.Username == senderUsername) ??
            throw new InvalidOperationException($"Отправитель {senderUsername} не числится в участниках чата");

        string? text = messageElement.Value.Trim();
        string type = GetAttributeOrElementValue(messageElement, "Type", locations.Append("Message"));

        string dateTimeValue = GetAttributeOrElementValue(messageElement, "DateTime", locations.Append($"Message"));
        if (dateTimeValue.IsWhiteSpace())
            throw new InvalidOperationException($"Пустой элемент DateTime в элементе Message");

        DateTime dateTime;
        if (!DateTime.TryParse(dateTimeValue, out dateTime)) throw new InvalidOperationException($"Неудачный парсинг элемента DateTime в элементе Message");

        return new(sender) { Text = text, Type = type, DateTime = dateTime };
    }
}

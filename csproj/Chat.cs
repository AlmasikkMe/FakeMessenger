using System.Data;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FakeMessenger;
public class Chat
{
    public Chat(string chatName, string name)
    {
        ChatName = chatName;
        Name = name;

        _members = [];
        _messages = [];
    }
    public Chat(XElement xElement, List<User> users) : this(xElement, users.AsReadOnly()) { }
    public Chat(XElement xElement, IReadOnlyList<User> users)
    {
        ChatName = xElement.Element("ChatName")?.Value ?? throw new InvalidOperationException("Обязательный элемент ChatName не найден в элементе Chat");
        Name = xElement.Element("Name")?.Value ?? throw new InvalidOperationException($"Обязательный элемент Name не найден в элементе Chat {ChatName}");

        _members = [];
        XElement membersElement = xElement.Element("Members") ?? throw new InvalidOperationException($"Обязательный элемент Members не найден в элементе Chat {ChatName}");
        List<string> membersUsernames = membersElement
            .Elements("User")
            .Select(member => member.Value)
            .ToList();

        AddMembers(
            membersUsernames
            .Select(username => 
                users.FirstOrDefault(user => user.Username == username) ?? throw new InvalidOperationException($"Пользователь {username} из элемента Chat {ChatName} не найден")
                )
            .ToList());

        _messages = [];
        XElement? messagesElement = xElement.Element("Messages");
        if (messagesElement is not null)
        {
            foreach (var message in messagesElement.Elements("Message"))
            {
                string senderUsername = message.Element("Sender")?.Value ?? throw new InvalidOperationException($"Обязательный элемент Sender не найден в элементе Message из элемента Chat {ChatName}");
                User sender = _members.FirstOrDefault(member => member.Username == senderUsername) ?? throw new InvalidOperationException($"Отправитель {senderUsername} не числится в участниках чата {ChatName}");

                string? text = message.Element("Text")?.Value.Trim();
                string? type = message.Element("Type")?.Value.Trim();

                string dateTimeValue = (message.Element("DateTime") ?? throw new InvalidOperationException($"Обязательный элемент DateTime не найден в элементе Message из элемента Chat {ChatName}")).Value;
                if (dateTimeValue.IsWhiteSpace()) throw new InvalidOperationException($"Пустой элемент DateTime в элементе Message из элемента Chat {ChatName}");
                DateTime dateTime;
                if (!DateTime.TryParse(dateTimeValue, out dateTime)) throw new InvalidOperationException($"Неудачный парсинг элемента DateTime в элементе Message из элемента Chat {ChatName}");

                AddMessage(sender, text, type, dateTime);
            } 
            }
    }

    public string ChatName
    {
        get;
        set
        {
            string chatName = value.Trim();
            if (!chatName.StartsWith("@")) chatName = $"@{chatName}";

            if (chatName.Skip(1).All(char.IsAsciiLetterOrDigit))
            {
                if (chatName.Length - 1 is >= 5 and <= 20) field = chatName;
                else throw new ArgumentException("Уникальное имя чата должно содержать от 5 до 20 символов (не считая @)");
            }
            else throw new ArgumentException("Уникальное имя чата допускает только латинские символы или цифры");
        }
    }
    public string Name
    {
        get;
        set => field = !value.IsWhiteSpace() ? value.Trim() : throw new ArgumentException("Название чата не может быть пустым");
    }
    private List<User> _members;
    private List<Message> _messages;

    public IReadOnlyList<User> Members => _members.AsReadOnly();
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

    public XElement ToXElement() =>
        new("Chat",
            new XAttribute("ChatName", ChatName),
            new XAttribute("Name", Name),
            new XElement("Members", from member in _members select new XElement("User", member.Username)),
            new XElement("Messages", from message in _messages select message.ToXElement())
            );

    public List<User> GetMembers(string search = "")
    {
        return (from member in Members
                where member.Username.Contains(search, StringComparison.OrdinalIgnoreCase)
                select member)
                .ToList();
    }

    public void AddMembers(List<User> members)
    {
        _members.AddRange(members.Except(_members).ToList());
    }

    public void AddMessage(User sender, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        Message message = new(sender);

        if (sender != null) message.Sender = sender;
        if (text != null) message.Text = text;
        if (type != null) message.Type = type;
        if (dateTime != null) message.DateTime = (DateTime)dateTime;

        _messages.Add(message);
        _messages = (from m in _messages
                    orderby m.DateTime
                    select m)
                    .ToList();
    }
}

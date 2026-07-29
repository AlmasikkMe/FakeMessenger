using System.Data;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FakeMessenger.Core;
public class Chat
{
    public Chat(string chatName, string name)
    {
        ChatName = chatName;
        Name = name;

        _members = [];
        _messages = [];
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

    public void AddMessage(Message message)
    {
        _messages.Add(message);
        _messages = (from m in _messages
                    orderby m.DateTime
                    select m)
                    .ToList();
    }

    public void AddMessage(User sender, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        Message message = new(sender);

        if (text != null) message.Text = text;
        if (type != null) message.Type = type;
        if (dateTime != null) message.DateTime = (DateTime)dateTime;

        AddMessage(message);
    }
}

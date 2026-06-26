using System.Xml.Serialization;

namespace ConsoleFakeChat;
public class Chat(string chatName)
{
    public Chat() : this("Чат") { }

    public string ChatName = chatName;
    private List<User> _members = [];
    private List<Message> _messages = [];

    [XmlArray("Members")] [XmlArrayItem("Member")]  public List<User> Members { get => _members; }
    [XmlArray("Messages")] [XmlArrayItem("Message")]  public List<Message> Messages { get => _messages; }

    public List<User> GetMembers(string search = "")
    {
        return (from member in Members
                where member.Username.Contains(search, StringComparison.OrdinalIgnoreCase)
                select member)
                .ToList();
    }

    public void AddMembers(List<User> members)
    {
        Members.AddRange(members.Except(Members));
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

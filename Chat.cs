using System.Xml.Serialization;

public class Chat(string chatName)
{
    public Chat() : this("Чат") { }

    public string ChatName = chatName;
    private List<string> _members = [];
    private List<Message> _messages = [];

    [XmlArray("Members")] [XmlArrayItem("Member")]  public List<string> Members { get => _members; }
    [XmlArray("Messages")] [XmlArrayItem("Message")]  public List<Message> Messages { get => _messages; }

    public List<string> GetMembers(string search = "")
    {
        return (from member in Members
                where member.Contains(search, StringComparison.OrdinalIgnoreCase)
                select member)
                .ToList();
    }

    public void AddMembers(List<string> members)
    {
        Members.AddRange(members.Except(Members));
    }

    public void AddMessage(string? sender = null, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        Message message = new();

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

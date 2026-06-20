public class Chat(string chatName)
{
    public string ChatName = chatName;
    private List<string> Members = [];
    public List<Message> Messages = [];

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

        Messages.Add(message);
    }
}

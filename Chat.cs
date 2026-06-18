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
}

public class Messenger
{
    private List<string> Contacts = [];
    private List<Chat> Chats = [];

    public void NewContact(string contactName)
    {
        contactName = contactName.IsWhiteSpace() ? $"Контакт {Contacts.Count + 1}" : contactName.Trim();
        Contacts.Add(contactName);

        Chat chat = new(contactName);
        chat.AddMembers(["Вы", contactName]);

        Chats.Add(chat);
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

        Chats.Add(chat);
    }
    public List<Chat> GetChats(string search = "")
    {
        return (from chat in Chats
                where chat.ChatName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                select chat)
                .ToList();
    }
}

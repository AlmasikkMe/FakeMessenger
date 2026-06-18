public class Messenger
{
    private List<string> Contacts = [];
    private List<Chat> Chats = [];

    public void NewContact(string contactName)
    {
        contactName = contactName.IsWhiteSpace() ? $"Контакт {Contacts.Count + 1}" : contactName.Trim();
        Contacts.Add(contactName);
        Chats.Add(new(contactName) { Members = [contactName, "Вы"] });
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

        Chats.Add(new(groupName) { Members = membersList });
    }

    public List<Chat> GetChats(string? chatName = null)
    {
        if (string.IsNullOrWhiteSpace(chatName))
        {
            return Chats;
        }

        List<Chat> chats = Chats.Where(x => x.ChatName.ToLower().Contains(chatName.Trim().ToLower())).ToList();
        return chats;
    }
}

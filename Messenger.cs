public class Messenger
{
    public static Dictionary<string, (string Emoji, string Name, bool IsWithTime)> MessagesTypes = new()
    {
        { "photo",        ("🖼", "Фотография",          false) },
        { "video",        ("📹", "Видеозапись",         false) },
        { "video_note",   ("📹", "Видеосообщение",      true)  },
        { "gif",          ("💥", "GIF",                 false) },
        { "voice",        ("🎤", "Голосовое сообщение", true)  },
        { "audio",        ("🎶", "Аудиозапись",         false) },
        { "voice_effect", ("🔊", "Голосовой эффект",    false) },
        { "file",         ("📂", "Файл",                false) },
        { "sticker",      ("🎨", "Стикер",              false) },
        { "emoji",        ("🎨", "Эмодзи",              false) },
        { "poll",         ("📊", "Опрос",               false) },
        { "quiz",         ("📊", "Викторина",           false) },
        { "contact",      ("👤", "Контакт",             false) },
        { "location",     ("📍", "Геолокация",          false) },
        { "live_location",("🚨", "Живая геолокация",    false) },
        { "game",         ("🎮", "Игра",                false) },
        { "product",      ("🛍", "Продукт",             false) },
        { "gift",         ("🎁", "Подарок",             false) }
    };
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

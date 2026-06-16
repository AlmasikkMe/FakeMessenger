using System.Globalization;

Messager messager = new();
while (true)
{
    Console.Clear();
    Console.WriteLine("Что вы хотите сделать?");
    Console.WriteLine("1. Создать контакт");
    Console.WriteLine("2. Создать группу");
    Console.WriteLine("3. Перейти в чат");

    switch (Console.ReadLine())
    {
        case "1":
            NewContact();
            break;
        case "2":
            NewGroup();
            break;
        case "3":
            ViewChatHistory(ChooseChat());
            break;
    }
}

void NewContact()
{
    Console.Write("Введите имя для контакта: ");
    string? contact = Console.ReadLine();
    if (contact == null) Console.WriteLine();
    messager.NewContact(contact ?? "");
}

void NewGroup()
{
    Console.Write("Выберите имена членов группы (через запятую):");

    string[] members;
    string? userInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(userInput))
    {
        members = userInput.Split(",");
    }

    Console.Write("Введите имя для группы: ");
    userInput = Console.ReadLine();
    if (userInput == null) Console.WriteLine();
}

Chat ChooseChat()
{
    Console.Clear();
    Console.Write("Напишите имя чата для поиска:");

    List<Chat> chats = messager.GetChats(Console.ReadLine());
    chats.ForEach(chat => Console.WriteLine(chat.ChatName));

    Console.CursorVisible = false;
    (int left, int top) = (Console.CursorLeft, Console.CursorTop);
    int selectIndex = 0;

    while (true)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        Console.SetCursorPosition(0, top - chats.Count + selectIndex);
        Console.Write(chats[selectIndex].ChatName);

        Console.ResetColor();

        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.UpArrow:
                if (selectIndex > 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(chats[selectIndex].ChatName);
                    selectIndex--;
                }
                break;

            case ConsoleKey.DownArrow:
                if (selectIndex < chats.Count - 1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(chats[selectIndex].ChatName);
                    selectIndex++;
                }
                break;

            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter:
                Console.SetCursorPosition(left, top);
                Console.CursorVisible = false;
                return chats[selectIndex];
        }
    }
}

void ViewChatHistory(Chat chat)
{
    chat.Messages.ForEach(message => 
    {
        Console.WriteLine($"{message.Sender}, {message.DateTime:dd.MM.yyyy HH:mm}");
        if (message.Type != "text")
        {
            Console.Write($"[{Messager.MessagesTypes[message.Type].Emoji} ");
            if (message.Text == string.Empty) Console.WriteLine($"{Messager.MessagesTypes[message.Type].Name}]");
            else Console.WriteLine($"{message.Text}]");
        }
        Console.WriteLine(message.Text);
    });
}


public class Messager
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
        Chats.Add(new(contactName) { Members = [contactName] });
    }

    public void NewGroup(string groupName, string[] members)
    {
        if (members.Length == 0) return;

        List<string> membersList = [];

        for (int i = 0; i < members.Length; i++)
        {
            if (!members[i].IsWhiteSpace()) membersList.Add(members[i].Trim());
        }

        if (groupName.IsWhiteSpace()) 
        {
            groupName = "";

            for (int i = 0; i < 5 && i < membersList.Count; i++)
            {
                if (i == 4)
                {
                    groupName += $"еще {membersList.Count - i + 1}";
                    break;
                }

                groupName += membersList[i];

                if (i >= membersList.Count - 1) groupName += ", ";
            }
        }

        Chats.Add(new(groupName) { Members = membersList });
    }

    public List<Chat> GetChats(string? chatName)
    {
        if (string.IsNullOrWhiteSpace(chatName))
        {
            return Chats;
        }

        List<Chat> chats = Chats.Where(x => x.ChatName.ToLower().Contains(chatName.Trim().ToLower())).ToList();
        return chats;
    }
}

public class Chat(string chatName)
{
    public string ChatName = chatName;
    public List<string> Members = [];
    public List<Message> Messages = [];
}

public class Message(string sender, string text, string type = "text")
{
    public string Sender = sender;
    public string Text = text;
    public string Type = type;
    public DateTime DateTime;
}

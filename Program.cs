using System.Globalization;

Messenger messenger = new();
while (true)
{
    Console.Clear();
    Console.WriteLine("Что вы хотите сделать?");

    Console.WriteLine("1. Создать контакт");
    Console.WriteLine("2. Создать группу");
    Console.WriteLine("3. Перейти в чат");
    Console.WriteLine("0. Выйти");

    switch (Console.ReadLine())
    {
        case "1":
            NewContact();
            break;
        case "2":
            NewGroup();
            break;
        case "3":
            ChatCommandMenu(ChooseChat());
            break;
        case "0":
            return;
    }
}

void NewContact()
{
    Console.Write("Введите имя для контакта: ");
    string? contact = Console.ReadLine();
    if (contact == null) Console.WriteLine();
    messenger.NewContact(contact ?? "");
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

    List<Chat> chats = messenger.GetChats(Console.ReadLine());
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
            Console.Write($"[{Messenger.MessagesTypes[message.Type].Emoji} ");
            if (message.Text == string.Empty) Console.WriteLine($"{Messenger.MessagesTypes[message.Type].Name}]");
            else Console.WriteLine($"{message.Text}]");
        }
        Console.WriteLine(message.Text);
    });
}

void SendMessage(Chat chat)
{
    string? text = null;
    while (string.IsNullOrWhiteSpace(text))
    {
        Console.Write("Введите сообщение: ");
        text = Console.ReadLine();
    }
    text = text.Trim();

    string? sender;
    while (true)
    {
        Console.Write("Введите отправителя: ");
        sender = Console.ReadLine();
        sender ??= "";
        sender = sender.Trim();
        if (chat.Members.Contains(sender)) break;
        else Console.WriteLine($"Участники данного чата: {string.Join(", ", chat.Members)}");
    }
      
    string? type;
    while (true)
    {
        Console.Write("Введите тип сообщения: ");
        type = Console.ReadLine();
        type ??= "";
        type = type.Trim();
        if (Messenger.MessagesTypes.Keys.Contains(type) || type == "text") break;
        else Console.WriteLine($"Доступные типы: {string.Join(", ", Messenger.MessagesTypes.Keys)}");
    }

    Message message = new(sender, text, type);

    Console.Write("Введите время сообщения: ");
    DateTime.TryParse(Console.ReadLine(), out message.DateTime);

    chat.Messages.Add(message);
}

void ChatCommandMenu(Chat chat)
{
    bool inDialog = true;
    while (inDialog)
    {
        Console.Clear();
        Console.WriteLine($"Вы в чате {chat.ChatName}");
        Console.WriteLine("Что вы хотите сделать?");

        Console.WriteLine("1. Посмотреть историю чата");
        Console.WriteLine("2. Отправить сообщение");
        Console.WriteLine("0. Выйти из чата");

        switch (Console.ReadLine())
        {
            case "1":
                ViewChatHistory(chat);
                Console.ReadLine();
                break;
            case "2":
                SendMessage(chat);
                break;
            case "0":
                inDialog = false;
                break;
        }
    }
}

string SearchDialog(Func<string, List<string>> optionsUpdate)
{
    Console.Clear();

    Console.CursorVisible = false;
    int selectIndex = -1;
    int offset = 0;

    Console.WriteLine("Напишите для поиска: ");
    string searchText = "";

    List<string> options = optionsUpdate(searchText);
    options.Take(Console.WindowHeight - 2)
        .ToList()
        .ForEach(option => Console.WriteLine(option));

    while (true)
    {
        Console.SetCursorPosition(0, selectIndex - offset + 1);

        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        if (selectIndex != -1) Console.Write(options[selectIndex]);
        else Console.Write("Напишите для поиска: ");

        Console.ResetColor();

        ConsoleKeyInfo consoleKey = Console.ReadKey(true);

        switch (consoleKey.Key)
        {
            case ConsoleKey.UpArrow:
                if (selectIndex > -1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(options[selectIndex]);
                    selectIndex--;

                    if (offset > 0)
                    {
                        offset--;

                        Console.Clear();
                        Console.WriteLine("Напишите для поиска: ");
                        options.Skip(offset)
                            .Take(Console.WindowHeight - 2)
                            .ToList()
                            .ForEach(option => Console.WriteLine(option));
                    }
                }
                break;

            case ConsoleKey.DownArrow:
                if (selectIndex < options.Count - 1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    if (selectIndex != -1) Console.Write(options[selectIndex]);
                    else Console.Write("Напишите для поиска: ");

                    selectIndex++;
                    if (selectIndex - offset + 1 >= Console.WindowHeight - 1)
                    {
                        offset++;

                        Console.Clear();
                        Console.WriteLine("Напишите для поиска: ");
                        options.Skip(offset)
                            .Take(Console.WindowHeight - 2)
                            .ToList()
                            .ForEach(option => Console.WriteLine(option));
                    }
                }
                break;

            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter:
                Console.CursorVisible = true;
                if (selectIndex == -1)
                {
                    Console.Clear();
                    Console.Write("Напишите для поиска: ");

                    string? input = Console.ReadLine();
                    if (input == null) Console.WriteLine();
                    searchText = input ?? "";

                    options = optionsUpdate(searchText);
                    options.ForEach(option => Console.WriteLine(option));

                    Console.CursorVisible = false;
                    break;
                }
                else
                {
                    return options[selectIndex];
        }
    }
}
}

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
    public DateTime DateTime = DateTime.Now;
}

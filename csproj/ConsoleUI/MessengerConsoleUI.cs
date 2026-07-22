namespace FakeMessenger.ConsoleUI;

public static class MessengerConsoleUI
{
    static Dictionary<string, (string Emoji, string Name, bool IsWithTime, bool IsWithText)> MessagesTypes = new() {
        { "photo",        ("🖼", "Фотография",          false, true ) },
        { "video",        ("📹", "Видео",               false, true ) },
        { "video_note",   ("📹", "Видеосообщение",      true,  false) },
        { "gif",          ("📹", "GIF Анимация",        false, true ) },
        { "voice",        ("🎤", "Голосовое сообщение", true,  false) },
        { "audio",        ("🎶", "Аудиозапись",         false, false) },
        { "file",         ("📂", "Файл",                false, true ) },
        { "sticker",      ("🎨", "Стикер",              false, false) },
        { "poll",         ("📊", "Опрос",               false, true ) },
        { "quiz",         ("📊", "Викторина",           false, true ) },
        { "contact",      ("👤", "Контакт",             false, false) },
        { "location",     ("📍", "Геолокация",          false, false) },
        { "live_location",("🚨", "Живая геолокация",    false, false) },
        { "gift",         ("🎁", "Подарок",             false, false) },
    };

    public static Messenger Messenger = new();

    public static void ViewChatHistory(Chat chat)
    {
        for (int i = 0; i < chat.Messages.Count; i++)
        {
            if (i != 0) Console.WriteLine();

            Message? message = chat.Messages[i];
            Console.WriteLine($"{message.Sender.FullName}, [{message.DateTime:dd.MM.yyyy HH:mm}]");
            if (message.Type != "text")
            {
                Console.Write($"[{MessagesTypes[message.Type].Emoji}  ");
                if (message.Text == string.Empty) Console.WriteLine($"{MessagesTypes[message.Type].Name}]");
                else Console.WriteLine($"{message.Text}]");
            }
            if (!message.Text.IsWhiteSpace()) Console.WriteLine(message.Text);
        }
    }

    public static void NewContact()
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        Console.Write("Введите имя пользователя для контакта (только латинские символы и цифры): ");
        string? username = Console.ReadLine();
        if (username == ConsoleUI.ExitCommand) return;

        Console.Write("Введите имя для контакта: ");
        string? firstName = Console.ReadLine();
        if (username == ConsoleUI.ExitCommand) return;

        Console.Write("Введите фамилию для контакта: ");
        string? lastName = Console.ReadLine();
        if (lastName == ConsoleUI.ExitCommand) return;

        Messenger.NewContact(username ?? "", firstName ?? "", lastName ?? "");
    }

    public static void NewGroup()
    {
        List<User> members = [];
        bool isChooseMembers = true;

        while (isChooseMembers)
        {
            var searchQuery = ConsoleUI.SearchDialog(search =>
                Messenger.GetContacts(search)
                         .Except(members)
                         .Select(member => member.Username)
                         .ToList(),
            "Выберите членов группы");

            User contact = Messenger.Contacts.First(contact => contact.Username == searchQuery);
            members.Add(contact);

            if (Messenger.Contacts.Count == members.Count) isChooseMembers = false;

            bool isYNDialog = true;
            while (isChooseMembers && isYNDialog)
            {
                Console.Write("Продолжить добавление членов группы? (Y/n): ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.N: isChooseMembers = false; break;
                    case ConsoleKey.Y: isYNDialog = false; break;
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        Console.Write("Введите уникальное имя группы (только латинские символы и цифры): ");
        string? chatName = Console.ReadLine();
        if (chatName == ConsoleUI.ExitCommand) return;
        chatName ??= "";

        Console.Write("Введите название группы: ");
        string? groupName = Console.ReadLine();
        if (groupName == ConsoleUI.ExitCommand) return;
        groupName ??= "";

        Messenger.NewGroup(chatName, groupName, members);
    }

    public static Chat ChooseChat()
    {
        string chatName = ConsoleUI.SearchDialog(searchText => Messenger.GetChats(searchText).Select(chat => chat.ChatName).ToList(), "Выберите чат");
        return Messenger.GetChats(chatName)[0];
    }

    public static void SendTextMessage(Chat chat)
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        string? text = null;
        while (string.IsNullOrWhiteSpace(text))
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine();
        }
        if (text == ConsoleUI.ExitCommand) return;
        text = text.Trim();

        string? sender = ConsoleUI.SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");


        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");

        string? userInput = Console.ReadLine();
        if (userInput == ConsoleUI.ExitCommand) return;
        
        DateTime.TryParse(userInput, out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;


        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        dateTime: dateTime);
    }

    public static void SendMultimediaMessage(Chat chat)
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        string? type = ConsoleUI.SearchDialog(searchText => MessagesTypes.Select(type => type.Key).ToList(), "Выберите тип сообщения");

        string? text = null;
        if (MessagesTypes[type].IsWithText)
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine() ?? "";
            if (text == ConsoleUI.ExitCommand) return;
            text = text.Trim();
        }

        string? sender = ConsoleUI.SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");


        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");

        string? userInput = Console.ReadLine();
        if (userInput == ConsoleUI.ExitCommand) return;

        DateTime.TryParse(userInput, out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;


        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        type: type,
                        dateTime: dateTime);
    }

    public static void CreateContactChat()
    {
        string userName = ConsoleUI.SearchDialog(search => Messenger.GetContacts(search)
                                                                    .Where(contact => !Messenger.Contacts.Contains(contact))
                                                                    .Select(contact => contact.Username)
                                                                    .ToList(),
                                                                    "Выберете контакт");

        User contact = Messenger.Contacts.First(user => user.Username == userName);

        Chat chat = new(contact.Username, contact.FullName);
        Messenger.AddChat(chat);
    }

    public static void Save()
    {
        Messenger.Save();
        Console.WriteLine($"Сохранено в файл {Messenger.SaveFile}");

        Console.ReadKey(true);
    }

    public static void Load()
    {
        Messenger.Load(el => Console.WriteLine($"Не удалось найти элемент {el}"), (el, ex) => Console.WriteLine($"Не удалось загрузить элемент {el}: {ex.Message}"));
        Console.WriteLine("Загружено из xml файла");

        Console.ReadKey(true);
    }
}
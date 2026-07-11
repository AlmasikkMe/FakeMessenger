namespace ConsoleFakeChat.ConsoleUI;
public static class ConsoleUI
{
    public static void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        MainMenu();
    }

    public static void MainMenu()
    {
        while (true)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Что вы хотите сделать?");

                Console.WriteLine("1. Создать контакт");
                Console.WriteLine("2. Создать группу");
                Console.WriteLine("3. Перейти в чат");
                Console.WriteLine("4. Сохранить");
                Console.WriteLine("5. Загрузить");
                Console.WriteLine("6. Создать чат с контактом");
                Console.WriteLine("0. Выйти");

                switch (Console.ReadLine())
                {
                    case "1":
                        MessagerConsoleUI.NewContact();
                        break;
                    case "2":
                        MessagerConsoleUI.NewGroup();
                        break;
                    case "3":
                        ChatCommandMenu(MessagerConsoleUI.ChooseChat());
                        break;
                    case "4":
                        MessagerConsoleUI.Save();
                        break;
                    case "5":
                        MessagerConsoleUI.Load();
                        break;
                    case "6":
                        MessagerConsoleUI.CreateContactChat();
                        break;
                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    public static void ChatCommandMenu(Chat chat)
    {
        bool inDialog = true;
        while (inDialog)
        {
            try
            {
                Console.Clear();
                Console.WriteLine($"Вы в чате {chat.ChatName}");
                Console.WriteLine("Что вы хотите сделать?");

                Console.WriteLine("1. Посмотреть историю чата");
                Console.WriteLine("2. Отправить сообщение");
                Console.WriteLine("3. Отправить мультимедиа");
                Console.WriteLine("0. Выйти из чата");

                switch (Console.ReadLine())
                {
                    case "1":
                        MessagerConsoleUI.ViewChatHistory(chat);
                        Console.ReadLine();
                        break;
                    case "2":
                        MessagerConsoleUI.SendTextMessage(chat);
                        break;
                    case "3":
                        MessagerConsoleUI.SendMultimediaMessage(chat);
                        break;
                    case "0":
                        inDialog = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    public static string SearchDialog(Func<string, List<string>> optionsUpdate, string? message = null)
    {
        bool isHasMessage = !string.IsNullOrWhiteSpace(message);

        string searchInstruction = "Напишите для поиска (ESC для выхода): ";

        Console.Clear();

        Console.CursorVisible = false;
        int selectIndex = -1;
        int offset = 0;

        if (isHasMessage) Console.WriteLine(message);

        Console.WriteLine(searchInstruction);
        string searchText = "";

        List<string> options = optionsUpdate(searchText);
        options.Take(Console.WindowHeight - (isHasMessage ? 3 : 2))
               .ToList()
               .ForEach(option => Console.WriteLine(option));

        while (true)
        {
            if (options.Count is 0) throw new ArgumentException("Нет списка для выбора");

            Console.SetCursorPosition(0, selectIndex - offset + (isHasMessage ? 2 : 1));

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (selectIndex != -1) Console.Write(options[selectIndex]);
            else Console.Write(searchInstruction);

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
                            if (isHasMessage) Console.WriteLine(message);
                            Console.WriteLine(searchInstruction);
                            options.Skip(offset)
                                .Take(Console.WindowHeight - (isHasMessage ? 3 : 2))
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
                        else Console.Write(searchInstruction);

                        selectIndex++;
                        if (selectIndex - offset + 1 >= Console.WindowHeight - 1)
                        {
                            offset++;

                            Console.Clear();
                            if (isHasMessage) Console.WriteLine(message);
                            Console.WriteLine(searchInstruction);
                            options.Skip(offset)
                                .Take(Console.WindowHeight - (isHasMessage ? 3 : 2))
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
                        if (isHasMessage) Console.WriteLine(message);
                        Console.Write(searchInstruction);

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
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    throw new OperationCanceledException("Отмена выбора");
            }
        }
    }
}

public static class MessagerConsoleUI
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
        Console.Clear();
        foreach (var message in chat.Messages)
        {
            Console.WriteLine($"{message.Sender.FullName}, [{message.DateTime:dd.MM.yyyy HH:mm}]");
            if (message.Type != "text")
            {
                Console.Write($"[{MessagesTypes[message.Type].Emoji}  ");
                if (message.Text == string.Empty) Console.WriteLine($"{MessagesTypes[message.Type].Name}]");
                else Console.WriteLine($"{message.Text}]");
            }
            Console.WriteLine(message.Text);
            Console.WriteLine();
        }
    }

    public static void NewContact()
    {
        Console.Write("Введите имя пользователя для контакта (только латинские символы и цифры): ");
        string? username = Console.ReadLine();

        Console.Write("Введите имя для контакта: ");
        string? firstName = Console.ReadLine();

        Console.Write("Введите фамилию для контакта: ");
        string? lastName = Console.ReadLine();

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

            bool isYNDialog = true;
            while (isChooseMembers && isYNDialog)
            {
                Console.Clear();
                Console.Write("Продолжить добавление членов группы? (Y/n): ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.N: isChooseMembers = false; break;
                    case ConsoleKey.Y: isYNDialog = false; break;
                }
            }
        }

        Console.Write("Введите уникальное имя группы (только латинские символы и цифры): ");
        string? chatName = Console.ReadLine();
        chatName ??= "";

        Console.Write("Введите название группы: ");
        string? groupName = Console.ReadLine();
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
        string? text = null;
        while (string.IsNullOrWhiteSpace(text))
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine();
        }
        text = text.Trim();

        string? sender = ConsoleUI.SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");
        Console.Clear();

        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");
        DateTime.TryParse(Console.ReadLine(), out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;

        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        dateTime: dateTime);
    }

    public static void SendMultimediaMessage(Chat chat)
    {
        string? type = ConsoleUI.SearchDialog(searchText => MessagesTypes.Select(type => type.Key).ToList(), "Выберите тип сообщения");
        Console.Clear();

        string? text = null;
        if (MessagesTypes[type].IsWithText)
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine() ?? "";
            text = text.Trim();
        }

        string? sender = ConsoleUI.SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");
        Console.Clear();

        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");
        DateTime.TryParse(Console.ReadLine(), out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;

        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        type: type,
                        dateTime: dateTime);
    }

    public static void CreateContactChat()
    {
        string contactName = ConsoleUI.SearchDialog(search => Messenger.GetContacts(search)
                                                                                           .Where(contact => !contact.IsHasChat)
                                                                                           .Select(contact => contact.Username)
                                                                                           .ToList(),
                                                                                           "Выберете контакт");
        
        Chat chat = Messenger.GetContacts(contactName).First(contact => contact.Username == contactName).Chat;
        Messenger.AddChat(chat);
    }

    public static void Save()
    {
        try
        {
            Messenger.Save();
            Console.WriteLine("Сохранено в файл Save.Messager.xml");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось сохранить в файл Save.Messager.xml: {ex.ToString()}");
        }

        Console.ReadLine();
    }

    public static void Load()
    {
        try
        {
            Messenger.Load(el => Console.WriteLine($"Не удалось найти элемент {el}"), (el, ex) => Console.WriteLine($"Не удалось загрузить элемент {el}: {ex.Message}"));
            Console.WriteLine("Загружено из файла Save.Messager.xml");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось загрузить из файла Save.Messager.xml: {ex.ToString()}");
        }

        Console.ReadLine();
    }
}
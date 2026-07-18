using System.ComponentModel.Design;
using System.Numerics;

namespace ConsoleFakeChat.ConsoleUI;
public static class ConsoleUI
{
    public static void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        MainMenu();
    }

    public static void ShowMenu(string? message, Dictionary<string, Action> menuActions, string exitOption = "Выйти")
    {
        bool isInMenu = true;
        menuActions.Add(exitOption, () => isInMenu = false);
        while (isInMenu)
        {
            try
            {
                string selected = SearchDialog(search => menuActions.Keys.Where(option => option.Contains(search)).ToList(), message);

                Action selectedAction = menuActions[selected];

                selectedAction();

                if (Console.CursorLeft != 0) Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    public static void MainMenu()
    {
        string? message = null;

        Dictionary<string, Action> menuActions = new()
        {
            { "Создать контакт", MessagerConsoleUI.NewContact },
            { "Создать группу", MessagerConsoleUI.NewGroup },
            { "Перейти в чат", () => ChatCommandMenu(MessagerConsoleUI.ChooseChat()) },
            { "Сохранить", MessagerConsoleUI.Save },
            { "Загрузить", MessagerConsoleUI.Load },
            { "Создать чат с контактом", MessagerConsoleUI.CreateContactChat },
        };

        ShowMenu(message, menuActions, "Выйти без сохранения");
    }

    public static void ChatCommandMenu(Chat chat)
    {
        string message = $"Вы в чате {chat.ChatName}";

        Dictionary<string, Action> menuActions = new()
        {
            { "Посмотреть историю чата", () => { MessagerConsoleUI.ViewChatHistory(chat); Console.ReadKey(true); } },
            { "Отправить сообщение", () => MessagerConsoleUI.SendTextMessage(chat) },
            { "Отправить мультимедиа", () => MessagerConsoleUI.SendMultimediaMessage(chat) },
        };

        ShowMenu(message, menuActions, "Выйти из чата");
    }

    public static string SearchDialog(Func<string, List<string>> optionsUpdate, string? message = null)
    {
        bool isHasMessage = !string.IsNullOrWhiteSpace(message);
        if (isHasMessage) Console.Write($"{message}: ");

        string searchInstruction = "Напишите для поиска (ESC для выхода): ";

        Console.Write("\x1b[?1049h"); // Включение альтернативного буфера
        bool isInAltBuffer = true;

        Console.SetCursorPosition(0, 0);

        try
        {
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

                Console.Write("\u001b[30;47m"); // Черный текст на белом фоне

                if (selectIndex != -1) Console.Write(options[selectIndex]);
                else Console.Write(searchInstruction);

                Console.Write("\u001b[0m"); // Сброс цветов

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
                        if (selectIndex == -1)
                        {
                            if (isHasMessage) Console.WriteLine(message);
                            Console.Write(searchInstruction);

                            Console.CursorVisible = false;
                            string? input = Console.ReadLine();
                            Console.CursorVisible = true;

                            if (input == null) Console.WriteLine();
                            searchText = input ?? "";

                            options = optionsUpdate(searchText);
                            options.ForEach(option => Console.WriteLine(option));
                            break;
                        }
                        else
                        {
                            Console.Write("\x1b[?1049l");
                            isInAltBuffer = false;

                            string selected = options[selectIndex];

                            Console.WriteLine(selected); // Вот это не выводится
                            return selected;
                        }
                    case ConsoleKey.Escape:
                        throw new OperationCanceledException("Отмена выбора");
                }
            }
        }
        finally
        {
            Console.CursorVisible = true;
                            
            if (isInAltBuffer) Console.Write("\x1b[?1049l"); 
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
                Console.Write("Продолжить добавление членов группы? (Y/n): ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.N: isChooseMembers = false; break;
                    case ConsoleKey.Y: isYNDialog = false; break;
                }
                Console.WriteLine();
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

        string? text = null;
        if (MessagesTypes[type].IsWithText)
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine() ?? "";
            text = text.Trim();
        }

        string? sender = ConsoleUI.SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");

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

        Console.ReadKey(true);
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

        Console.ReadKey(true);
    }
}
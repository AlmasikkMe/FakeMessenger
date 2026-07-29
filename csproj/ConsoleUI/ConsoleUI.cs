using FakeMessenger.Core;
using FakeMessenger.FileRepository.Xml;
using System.ComponentModel.Design;
using System.Numerics;

namespace FakeMessenger.ConsoleUI;
public class ConsoleUI(Messenger messenger)
{
    Dictionary<string, (string Emoji, string Name, bool IsWithTime, bool IsWithText)> MessagesTypes = new() {
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

    private Messenger _messenger = messenger;

    public string ExitCommand = "/";
    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        MainMenu();
    }

    public void ShowMenu(string? message, Dictionary<string, Action> menuActions, string exitOption = "Выйти")
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

    public void MainMenu()
    {
        string? message = null;

        Dictionary<string, Action> menuActions = new()
        {
            { "Создать контакт", NewContact },
            { "Создать группу", NewGroup },
            { "Перейти в чат", () => ChatCommandMenu(ChooseChat()) },
            { "Сохранить",  Save },
            { "Загрузить", Load },
            { "Создать чат с контактом", CreateContactChat },
        };

        ShowMenu(message, menuActions, "Выйти без сохранения");
    }

    public void ChatCommandMenu(Chat chat)
    {
        string message = $"Вы в чате {chat.ChatName}";

        Dictionary<string, Action> menuActions = new()
        {
            { "Посмотреть историю чата", () => { ViewChatHistory(chat); Console.ReadKey(true); } },
            { "Отправить сообщение", () => SendTextMessage(chat) },
            { "Отправить мультимедиа", () => SendMultimediaMessage(chat) },
        };

        ShowMenu(message, menuActions, "Выйти из чата");
    }

    public string SearchDialog(Func<string, List<string>> optionsUpdate, string? message = null)
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

    public void ViewChatHistory(Chat chat)
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

    public void NewContact()
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        Console.Write("Введите имя пользователя для контакта (только латинские символы и цифры): ");
        string? username = Console.ReadLine();
        if (username == ExitCommand) return;

        Console.Write("Введите имя для контакта: ");
        string? firstName = Console.ReadLine();
        if (username == ExitCommand) return;

        Console.Write("Введите фамилию для контакта: ");
        string? lastName = Console.ReadLine();
        if (lastName == ExitCommand) return;

        _messenger.NewContact(username ?? "", firstName ?? "", lastName ?? "");
    }

    public void NewGroup()
    {
        List<User> members = [];
        bool isChooseMembers = true;

        while (isChooseMembers)
        {
            var searchQuery = SearchDialog(search =>
                _messenger.GetContacts(search)
                         .Except(members)
                         .Select(member => member.Username)
                         .ToList(),
            "Выберите членов группы");

            User contact = _messenger.Contacts.First(contact => contact.Username == searchQuery);
            members.Add(contact);

            if (_messenger.Contacts.Count == members.Count) isChooseMembers = false;

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
        if (chatName == ExitCommand) return;
        chatName ??= "";

        Console.Write("Введите название группы: ");
        string? groupName = Console.ReadLine();
        if (groupName == ExitCommand) return;
        groupName ??= "";

        _messenger.NewGroup(chatName, groupName, members);
    }

    public Chat ChooseChat()
    {
        string chatName = SearchDialog(searchText => _messenger.GetChats(searchText).Select(chat => chat.ChatName).ToList(), "Выберите чат");
        return _messenger.GetChats(chatName)[0];
    }

    public void SendTextMessage(Chat chat)
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        string? text = null;
        while (string.IsNullOrWhiteSpace(text))
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine();
        }
        if (text == ExitCommand) return;
        text = text.Trim();

        string? sender = SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");


        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");

        string? userInput = Console.ReadLine();
        if (userInput == ExitCommand) return;

        DateTime.TryParse(userInput, out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;


        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        dateTime: dateTime);
    }

    public void SendMultimediaMessage(Chat chat)
    {
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        string? type = SearchDialog(searchText => MessagesTypes.Select(type => type.Key).ToList(), "Выберите тип сообщения");

        string? text = null;
        if (MessagesTypes[type].IsWithText)
        {
            Console.Write("Введите сообщение: ");
            text = Console.ReadLine() ?? "";
            if (text == ExitCommand) return;
            text = text.Trim();
        }

        string? sender = SearchDialog(search => chat.GetMembers(search).Select(member => member.Username).ToList(), "Выберите отправителя");


        DateTime dateTime = DateTime.Now;
        Console.Write("Введите время сообщения: ");

        string? userInput = Console.ReadLine();
        if (userInput == ExitCommand) return;

        DateTime.TryParse(userInput, out dateTime);
        if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;


        chat.AddMessage(sender: chat.GetMembers().First(member => member.Username == sender),
                        text: text,
                        type: type,
                        dateTime: dateTime);
    }

    public void CreateContactChat()
    {
        string userName = SearchDialog(search => _messenger.GetContacts(search)
                                                                    .Where(contact => !_messenger.Contacts.Contains(contact))
                                                                    .Select(contact => contact.Username)
                                                                    .ToList(),
                                                                    "Выберете контакт");

        User contact = _messenger.Contacts.First(user => user.Username == userName);

        Chat chat = new(contact.Username, contact.FullName);
        _messenger.AddChat(chat);
    }

    public void Save()
    {
        _messenger.Save();
        Console.WriteLine($"Сохранено в xml файл");

        Console.ReadKey(true);
    }

    public void Load()
    {
        _messenger.Load();
        Console.WriteLine("Загружено из xml файла");

        Console.ReadKey(true);
    }
}
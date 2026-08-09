using FakeMessenger.Core;

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
        { "u",      ("👤", "Контакт",             false, false) },
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
                string selected = new SearchDialog(search => menuActions.Keys.Where(option => option.Contains(search)).ToList(), message).Show();

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
            { "Удалить чат", () => _messenger.RemoveChat(ChooseChat("Выберите чат для удаления")) },
            { "Удалить контакт", () => _messenger.RemoveContact(ChooseContact("Выберите контакт для удаления")) },
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
            { "Отправить сообщение", () => SendMessage(chat, type: "text") },
            { "Отправить мультимедиа", () => SendMessage(chat) },
        };

        ShowMenu(message, menuActions, "Выйти из чата");
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
            User contact = ChooseContact("Добавьте члена группы", members);
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

    private Chat ChooseChat(string message = "Выберите чат", List<Chat>? excludedChats = null)
    {
        string chatName = new SearchDialog(searchText => _messenger.GetChats(searchText)
                                                               .Except(excludedChats ?? [])
                                                               .Select(chat => chat.ChatName)
                                                               .ToList(), message).Show();
        return _messenger.Chats.First(c => c.ChatName == chatName);
    }

    private User ChooseContact(Func<string, List<User>> usersUpdate, string message = "Выбурите контакт")
    {
        string username = new SearchDialog(searchText => usersUpdate(searchText).Where(u => !u.IsDeleted)
                                                                            .Select(u => u.Username)
                                                                            .ToList(), message).Show();

        if (username == _messenger.User.Username) return _messenger.User;

        return _messenger.Contacts.First(u => u.Username == username);
    }

    private User ChooseContact(string message = "Выберите контакт", List<User>? excludedUsers = null)
    {
        return ChooseContact(searchText => _messenger.GetContacts(searchText)
                                                     .Except(excludedUsers ?? [])
                                                     .ToList());
    }

    public void SendMessage(Chat chat, User? sender = null, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        string? userInput;
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        sender ??= ChooseContact(searchText => chat.GetMembers(searchText).Where(u => !u.IsDeleted).ToList(), "Выберете отправителя");

        while (string.IsNullOrWhiteSpace(text))
        {
            Console.Write("Введите сообщение: ");

            userInput = Console.ReadLine();
            if (userInput == ExitCommand) throw new OperationCanceledException("Выход");

            text = userInput;
        }

        type ??= new SearchDialog(searchText => MessagesTypes.Select(type => type.Key).ToList(), "Выберите тип сообщения").Show();

        if (dateTime is null)
        {
            Console.Write("Введите дату отправки: ");

            userInput = Console.ReadLine();
            if (userInput == ExitCommand) throw new OperationCanceledException("Выход");

            DateTime outDateTime;
            if (DateTime.TryParse(userInput, out outDateTime)) dateTime = outDateTime;
            if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;
        }

        chat.AddMessage(sender, text, type, dateTime);
    }

    [Obsolete("Используйте SendMessage(chat, type: \"text\")")]
    public void SendTextMessage(Chat chat)
    {
        SendMessage(chat, type: "text");
    }

    [Obsolete("Используйте SendMessage(chat)")]
    public void SendMultimediaMessage(Chat chat)
    {
        SendMessage(chat);
    }

    public void CreateContactChat()
    {
        User contact = ChooseContact(excludedUsers: _messenger.Contacts
                                                              .Where(u => _messenger.Chats.Any(c => c.ChatName == u.Username))
                                                              .ToList());

        Chat chat = new(contact.Username, contact.FullName);
        chat.AddMembers([contact, _messenger.User]);

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
using FakeMessenger.Core;
using System.Text;

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

    private bool _isInMenu = false;

    public void ShowMenu(string? message, Dictionary<string, Action> menuActions, string exitOption = "Выйти")
    {
        _isInMenu = true;
        menuActions.Add(exitOption, () => _isInMenu = false);
        while (_isInMenu)
        {
            try
            {
                string selected = new SearchDialog(menuActions.Keys.ToList()).Show();

                Action selectedAction = menuActions[selected];

                selectedAction();

                if (Console.CursorLeft != 0) Console.WriteLine();
                Console.WriteLine();
            }
            catch (OperationCanceledException)
            {
                _isInMenu = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        _isInMenu = true;
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

    public void MessageCommandMenu(Message message, Chat chat)
    {
        string prompt = $"Что вы хотите с выбранным сообщением?";

        Dictionary<string, Action> menuActions = new()
        {
            { "Посмотреть выбранное сообщение", () => { Console.WriteLine(MessageToString(message)); } },
            { "Удалить сообщение", () => { chat.DeleteMessage(message); _isInMenu = false; } },
        };

        ShowMenu(prompt, menuActions, "Вернуться в чат");
    }

    public void ViewChatHistory(Chat chat)
    {
        Dictionary<string, Message> messages = [];

        foreach (var message in chat.Messages)
        {  
            messages.Add(MessageToString(message), message);
        }

        Message selectedMessage = messages[new SearchDialog(messages.Keys.ToList(), "Выберите сообщение").Show()];

        MessageCommandMenu(selectedMessage, chat);

        messages.Clear();
    }

    private string MessageToString(Message message)
    {
        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine($"{message.Sender.FullName}, [{message.DateTime:dd.MM.yyyy HH:mm}]");

        if (message.Type != "text")
        {
            stringBuilder.Append($"[{MessagesTypes[message.Type].Emoji}  ");

            if (message.Text == string.Empty) stringBuilder.AppendLine($"{MessagesTypes[message.Type].Name}]");
            else stringBuilder.AppendLine($"{message.Text}]");
        }

        if (message.Text != string.Empty) stringBuilder.AppendLine(message.Text);

        return stringBuilder.ToString();
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
        List<string> chatsNames = _messenger.Chats
                                            .Select(c => c.ChatName)
                                            .ToList();

        string chatName = new SearchDialog(chatsNames, message).Show();
        return _messenger.Chats.First(c => c.ChatName == chatName);
    }

    private User ChooseUser(List<User> users, string message = "Выбурите контакт")
    {
        List<string> usernames = users.Where(u => !u.IsDeleted)
                                      .Select(u => u.Username)
                                      .ToList();

        string username = new SearchDialog(usernames, message).Show();

        if (username == _messenger.User.Username) return _messenger.User;

        return _messenger.Contacts.First(u => u.Username == username);
    }

    private User ChooseContact(string message = "Выберите контакт", List<User>? excludedUsers = null)
    {
        return ChooseUser(_messenger.Contacts
                                    .Except(excludedUsers ?? [])
                                    .ToList());
    }

    public void SendMessage(Chat chat, User? sender = null, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        string? userInput;
        Console.WriteLine("Введите / на любом из следующих этапов для выхода");

        sender ??= ChooseUser(chat.Members.Where(u => !u.IsDeleted).ToList(), "Выберете отправителя");

        while (string.IsNullOrWhiteSpace(text))
        {
            Console.Write("Введите сообщение: ");

            userInput = Console.ReadLine();
            if (userInput == ExitCommand) throw new OperationCanceledException("Выход");

            text = userInput;
        }

        type ??= new SearchDialog(MessagesTypes.Select(type => type.Key).ToList(), "Выберите тип сообщения").Show();

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
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

Messenger messenger = new();

Dictionary<string, (string Emoji, string Name, bool IsWithTime, bool IsWithText)> MessagesTypes = new()
{
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


while (true)
{
    Console.Clear();
    Console.WriteLine("Что вы хотите сделать?");

    Console.WriteLine("1. Создать контакт");
    Console.WriteLine("2. Создать группу");
    Console.WriteLine("3. Перейти в чат");
    Console.WriteLine("4. Сохранить");
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
        case "4":
            messenger.Save();
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
    else return;

    Console.Write("Введите имя для группы: ");
    userInput = Console.ReadLine();
    userInput ??= "";

    messenger.NewGroup(userInput, members);
}

Chat ChooseChat()
{
    string chatName = SearchDialog(searchText => messenger.GetChats(searchText).Select(chat => chat.ChatName).ToList());
    return messenger.GetChats(chatName)[0];
}

void ViewChatHistory(Chat chat)
{
    Console.Clear();
    chat.Messages.ForEach(message => 
    {
        Console.WriteLine($"{message.Sender}, [{message.DateTime:dd.MM.yyyy HH:mm}]");
        if (message.Type != "text")
        {
            Console.Write($"[{MessagesTypes[message.Type].Emoji}  ");
            if (message.Text == string.Empty) Console.WriteLine($"{MessagesTypes[message.Type].Name}]");
            else Console.WriteLine($"{message.Text}]");
        }
        Console.WriteLine(message.Text);
        Console.WriteLine();
    });
}

void SendTextMessage(Chat chat)
{
    string? text = null;
    while (string.IsNullOrWhiteSpace(text))
    {
        Console.Write("Введите сообщение: ");
        text = Console.ReadLine();
    }
    text = text.Trim();

    Console.Write("Выберите отправителя (Нажмите любую кнопку для продолжения): "); Console.ReadKey();
    string? sender = SearchDialog(chat.GetMembers);
    Console.Clear();

    DateTime dateTime = DateTime.Now;
    Console.Write("Введите время сообщения: ");
    DateTime.TryParse(Console.ReadLine(), out dateTime);
    if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;

    chat.AddMessage(sender: sender,
                    text: text,
                    dateTime: dateTime);
}

void SendMultimediaMessage(Chat chat)
{
    Console.Write("Выберите тип сообщения (Нажмите любую кнопку для продолжения): "); Console.ReadKey();
    string? type = SearchDialog(searchText => MessagesTypes.Select(type => type.Key).ToList());
    Console.Clear();

    string? text = null;
    if (MessagesTypes[type].IsWithText)
    {
        Console.Write("Введите сообщение: ");
        text = Console.ReadLine() ?? "";
        text = text.Trim();
    }

    Console.Write("Выберите отправителя (Нажмите любую кнопку для продолжения): "); Console.ReadKey();
    string? sender = SearchDialog(chat.GetMembers);
    Console.Clear();

    DateTime dateTime = DateTime.Now;
    Console.Write("Введите время сообщения: ");
    DateTime.TryParse(Console.ReadLine(), out dateTime);
    if (dateTime == DateTime.MinValue) dateTime = DateTime.Now;

    chat.AddMessage(sender: sender,
                    text: text,
                    type: type,
                    dateTime: dateTime);
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
        Console.WriteLine("3. Отправить мультимедиа");
        Console.WriteLine("0. Выйти из чата");

        switch (Console.ReadLine())
        {
            case "1":
                ViewChatHistory(chat);
                Console.ReadLine();
                break;
            case "2":
                SendTextMessage(chat);
                break;
            case "3":
                SendMultimediaMessage(chat);
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
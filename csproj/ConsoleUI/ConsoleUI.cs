using System.ComponentModel.Design;
using System.Numerics;

namespace FakeMessenger.ConsoleUI;
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
            { "Создать контакт", MessengerConsoleUI.NewContact },
            { "Создать группу", MessengerConsoleUI.NewGroup },
            { "Перейти в чат", () => ChatCommandMenu(MessengerConsoleUI.ChooseChat()) },
            { "Сохранить", MessengerConsoleUI.Save },
            { "Загрузить", MessengerConsoleUI.Load },
            { "Создать чат с контактом", MessengerConsoleUI.CreateContactChat },
        };

        ShowMenu(message, menuActions, "Выйти без сохранения");
    }

    public static void ChatCommandMenu(Chat chat)
    {
        string message = $"Вы в чате {chat.ChatName}";

        Dictionary<string, Action> menuActions = new()
        {
            { "Посмотреть историю чата", () => { MessengerConsoleUI.ViewChatHistory(chat); Console.ReadKey(true); } },
            { "Отправить сообщение", () => MessengerConsoleUI.SendTextMessage(chat) },
            { "Отправить мультимедиа", () => MessengerConsoleUI.SendMultimediaMessage(chat) },
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
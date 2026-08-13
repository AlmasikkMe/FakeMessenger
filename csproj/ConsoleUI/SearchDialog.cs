namespace FakeMessenger.ConsoleUI;
public class SearchDialog
{
    private List<string[]> _options;

    private int _selectIndex = -1;
    private int _offset = 0;
    private string _searchText = string.Empty;

    private bool _isInAltBuffer = false;

    private string _searchInstruction = "Напишите для поиска (/ для выхода): ";
    public string? Message
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) field = null;
            else field = value.Trim();
        }
    }
    public List<string> Options { get; set { field = value; UpdateOptions(); } }

    public SearchDialog(List<string> options, string? message = null)
    {
        Options = options;
        UpdateOptions();
        Message = message;
    }

    public string Show()
    {
        if (Message is not null) Console.Write($"{Message}: ");

        TurnOnAltBuffer();

        try
        {
            Console.CursorVisible = false;

            WriteAll();

            Console.SetCursorPosition(0, 0);

            while (true)
            {
                if (_options.Count is 0 && Options.Count is 0) throw new ArgumentException("Нет списка для выбора");

                WriteSelected();

                ConsoleKeyInfo consoleKey = Console.ReadKey(true);

                switch (consoleKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (_selectIndex > -1)
                        {
                            int cursorTop = Message is null ? 1 : 2;

                            foreach (var option in _options.Take(_selectIndex))
                            {
                                cursorTop += option.Length;
                            }

                            cursorTop -= _offset;

                            Console.SetCursorPosition(0, cursorTop);

                            foreach (var line in _options[_selectIndex])
                            {
                                Console.WriteLine(line);
                            }

                            _selectIndex--;

                            if (_offset > 0)
                            {
                                int lines = Message is null ? 1 : 2;

                                foreach (var option in _options.Take(_selectIndex + 1))
                                {
                                    lines += option.Length;
                                }

                                if (lines > Console.WindowHeight - 1) _offset = lines - Console.WindowHeight + 1;
                                else _offset = 0;
                                Console.Clear();
                                WriteAll();
                            }
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (_selectIndex < _options.Count - 1)
                        {
                            if (_selectIndex == -1)
                            {
                                WriteInstructionLine();
                            }
                            else
                            {
                                int cursorTop = Message is null ? 1 : 2;

                                foreach (var option in _options.Take(_selectIndex))
                                {
                                    cursorTop += option.Length;
                                }

                                cursorTop -= _offset;

                                Console.SetCursorPosition(0, cursorTop);

                                foreach (var line in _options[_selectIndex])
                                {
                                    Console.WriteLine(line);
                                }
                            }

                            _selectIndex++;

                            int lines = Message is null ? 1 : 2;

                            foreach (var option in _options.Take(_selectIndex + 1))
                            {
                                lines += option.Length;
                            }

                            lines -= _offset;

                            if (lines >= Console.WindowHeight - 1)
                            {
                                _offset = lines + _offset - Console.WindowHeight + 1;
                                Console.Clear();
                                WriteAll();
                            }
                        }
                        break;

                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        if (_selectIndex == -1)
                        {
                            Console.Clear();
                            WriteMessageLine();
                            Console.Write(_searchInstruction);

                            Console.CursorVisible = true;
                            string? input = Console.ReadLine();
                            Console.CursorVisible = false;

                            if (input == null) Console.WriteLine();
                            _searchText = input ?? string.Empty;
                            UpdateOptions();

                            WriteOptionsLines();

                            _selectIndex = 0;
                            _offset = 0;

                            Console.SetCursorPosition(0, 1);

                            break;
                        }
                        else
                        {
                            TurnOffAltBuffer();

                            string selected = Options[_selectIndex];

                            Console.WriteLine(selected);
                            return selected;
                        }
                    case ConsoleKey.Escape:
                    case ConsoleKey.Oem2:
                        throw new OperationCanceledException("Отмена выбора");
                }
            }
        }
        finally
        {
            Console.CursorVisible = true;

            TurnOffAltBuffer();
        }
    }

    private void TurnOnAltBuffer() { if (!_isInAltBuffer) { Console.Write("\x1b[?1049h"); _isInAltBuffer = true; } }
    private void TurnOffAltBuffer() { if (_isInAltBuffer) { Console.Write("\x1b[?1049l"); _isInAltBuffer = false; } }

    private void WriteSelected()
    {
        int cursorTop = Message is null ? 0 : 1;

        if (_selectIndex != -1)
        {
            cursorTop++;

            foreach (var option in _options.Take(_selectIndex))
            {
                cursorTop += option.Length;
            }

            cursorTop -= _offset;
        }

        Console.SetCursorPosition(0, cursorTop);

        Console.Write("\u001b[30;47m"); // Черный текст на белом фоне

        if (_selectIndex != -1)
        {
            foreach (var line in _options[_selectIndex])
            {
                Console.WriteLine(line);
            }
        }
        else Console.Write(_searchInstruction);

        Console.Write("\u001b[0m"); // Сброс цветов
    }

    private void WriteOptionsLines() 
    {
        int skipped = 0;

        _options.ToList()
                .ForEach(option => 
                {
                    foreach (var line in option)
                    {
                        if (skipped < _offset)
                        { 
                            skipped++;
                            continue;
                        } 

                        if (Console.CursorTop + 1 >= Console.WindowHeight) return;
                        Console.WriteLine(line);
                    }
                });
    }

    private void UpdateOptions()
    {
        _options = Options.Where(option => option.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                          .Select(SplitOption)
                          .ToList();

        static string[] SplitOption(string option) => option.Split('\n')
                                                     .SelectMany(l => l.Chunk(Console.WindowWidth))
                                                     .Select(c => new string(c).Trim())
                                                     .ToArray();
    }

    private void WriteMessageLine()
    {
        if (Message is not null) Console.WriteLine(Message); 
    }

    private void WriteInstructionLine()
    {
        Console.SetCursorPosition(0, Message is null ? 0 : 1);
        Console.WriteLine(_searchInstruction);
    }
        
    private void WriteAll()
    {
        Console.SetCursorPosition(0, 0);

        WriteMessageLine();
        WriteInstructionLine();
        WriteOptionsLines();
    }
}
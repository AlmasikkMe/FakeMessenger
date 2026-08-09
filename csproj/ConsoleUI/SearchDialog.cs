using System.Diagnostics;

namespace FakeMessenger.ConsoleUI;
public class SearchDialog
{
    private List<string> _options;

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
    public Func<string, List<string>> OptionsUpdate { get; set; }

    public SearchDialog(Func<string, List<string>> optionsUpdate, string? message = null)
    {
        OptionsUpdate = optionsUpdate;
        _options = OptionsUpdate(_searchText);
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

            while (true)
            {
                if (_options.Count is 0) throw new ArgumentException("Нет списка для выбора");

                WriteSelected();

                ConsoleKeyInfo consoleKey = Console.ReadKey(true);

                switch (consoleKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (_selectIndex > -1)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write(_options[_selectIndex]);
                            _selectIndex--;

                            if (_offset > 0)
                            {
                                _offset--;
                                WriteAll();
                            }
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (_selectIndex < _options.Count - 1)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            if (_selectIndex != -1) Console.Write(_options[_selectIndex]);
                            else Console.Write(_searchInstruction);

                            _selectIndex++;
                            if (_selectIndex - _offset + 1 >= Console.WindowHeight - 1)
                            {
                                _offset++;
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

                            Console.CursorVisible = false;
                            string? input = Console.ReadLine();
                            Console.CursorVisible = true;

                            if (input == null) Console.WriteLine();
                            _searchText = input ?? "";

                            WriteOptionsLines();
                            break;
                        }
                        else
                        {
                            TurnOffAltBuffer();

                            string selected = _options[_selectIndex];

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
        Console.SetCursorPosition(0, _selectIndex - _offset + (Message is null ? 1 : 2));

        Console.Write("\u001b[30;47m"); // Черный текст на белом фоне

        if (_selectIndex != -1) Console.Write(_options[_selectIndex]);
        else Console.Write(_searchInstruction);

        Console.Write("\u001b[0m"); // Сброс цветов
    }

    private void WriteOptionsLines()
    {
        _options = OptionsUpdate(_searchText);
        _options.Skip(_offset)
                .Take(Console.WindowHeight - (Message is null ? 2 : 3))
                .ToList()
                .ForEach(option => Console.WriteLine(option));
    }

    private void WriteMessageLine()
    {
        if (Message is not null) Console.WriteLine(Message);
    }

    private void WriteInstructionLine()
    {
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
using FakeMessenger;
using FakeMessenger.ConsoleUI;

Messenger messenger = new();

ConsoleUI consoleUI = new(messenger);

consoleUI.Run();
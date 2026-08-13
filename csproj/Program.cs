using FakeMessenger.ConsoleUI;
using FakeMessenger.Core;
using FakeMessenger.FileRepository.Xml;

XmlSerializer xmlSerializer = new();

XmlFileRepository xmlFileRepository = new(xmlSerializer);

Messenger messenger = new(xmlFileRepository);

ConsoleUI consoleUI = new(messenger);

List<string> strings = [
    "string\nlines",
    "1111111111111111111111111111111111111111111111111111111111111111111111",
    "22222222222222222222222222222222\n22222222222222222222222222222222222222",
    "3333333333333333333333\n3333333333333333333333333333\n33333333333333333333",
    "4444444444444\n44444444444444444444\n44444444444444444444\n44444444444444444"
    ];

new SearchDialog(strings.ToList()).Show();

consoleUI.Run();
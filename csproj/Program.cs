using FakeMessenger.ConsoleUI;
using FakeMessenger.Core;
using FakeMessenger.FileRepository.Xml;

XmlSerializer xmlSerializer = new();

XmlFileRepository xmlFileRepository = new(xmlSerializer);

Messenger messenger = new(xmlFileRepository);

ConsoleUI consoleUI = new(messenger);

consoleUI.Run();
using FakeMessenger;
using FakeMessenger.ConsoleUI;
using FakeMessenger.FileRepository.Xml;

MessengerXmlSerializer xmlSerializer = new();

MessengerXmlFileRepository xmlFileRepository = new(xmlSerializer);

Messenger messenger = new(xmlFileRepository);

ConsoleUI consoleUI = new(messenger);

consoleUI.Run();
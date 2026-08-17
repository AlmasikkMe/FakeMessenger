using FakeMessenger.Core;
using FakeMessenger.FileRepository.Xml;
using FakeMessenger.UI.WpfUI;
using FakeMessenger.UI.ConsoleUI;
using FakeMessenger.UI;

XmlSerializer xmlSerializer = new();

XmlFileRepository xmlFileRepository = new(xmlSerializer);

Messenger messenger = new(xmlFileRepository);

IUserInterface userInterface = new WpfUI(messenger);

userInterface.Run();
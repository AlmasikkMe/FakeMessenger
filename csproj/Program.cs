using FakeMessenger.ConsoleUI;
using FakeMessenger.Core;
using FakeMessenger.FileRepository.Xml;
using FakeMessenger.WpfUI;

XmlSerializer xmlSerializer = new();

XmlFileRepository xmlFileRepository = new(xmlSerializer);

Messenger messenger = new(xmlFileRepository);

ConsoleUI consoleUI = new(messenger);

// consoleUI.Run();

Thread wpfThread = new(() =>
{
    var app = new System.Windows.Application();
    MainMenu mainMenu = new MainMenu();
    app.Run(mainMenu);
});

wpfThread.SetApartmentState(ApartmentState.STA);

wpfThread.Start();
wpfThread.Join();
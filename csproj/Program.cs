using FakeMessenger.Services;
using FakeMessenger.Core.Repositories;
using FakeMessenger.FileRepository.Xml;
using FakeMessenger.UI.ConsoleUI;
using FakeMessenger.UI;

try
{
    XmlSerializer xmlSerializer = new();

    XmlFileRepository xmlFileRepository = new(xmlSerializer);
    UsersRepository usersRepository = new();
    ChatsRepository chatsRepository = new();

    Messenger messenger = new(xmlFileRepository, usersRepository, chatsRepository);

    IUserInterface userInterface = new ConsoleUI(messenger);

    userInterface.Run();
}
finally
{
#if WINDOWS
    System.Windows.Application app = System.Windows.Application.Current;
    app?.Dispatcher.Invoke(() => app.Shutdown());
#endif
}
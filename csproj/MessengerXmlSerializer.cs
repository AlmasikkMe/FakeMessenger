using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace FakeMessenger
{
    public static class MessengerXmlSerializer
    {
        public static List<FileInfo> OldSaveFiles { get; set; } = [new("Save.Messager.xml")];
        public static FileInfo SaveFile { get; set; } = new("Messenger.Save.xml");
        public static void SerializeAndSave(Messenger messenger)
        {
            XDocument doc = new(
                new XElement("Messenger",
                    messenger.User.ToXElement(),
                    new XElement("Contacts", from user in messenger.Contacts select user.ToXElement()),
                    new XElement("Chats", from chat in messenger.Chats select chat.ToXElement())
                    )
                );

            doc.Save(SaveFile.FullName);
        }

        private static InvalidOperationException LoadFailedException(string element) => new($"Не удалось получить элемент {element} в {SaveFile.FullName}");

        public static Messenger Deserialize()
        {
            User user;
            List<User> contacts;
            List<Chat> chats;

            XDocument doc = XDocument.Load(SaveFile.FullName);

            if (doc.Root is null) throw new InvalidOperationException($"Не удалось получить корневой элемент в {SaveFile.FullName}");

            XElement userElement = doc.Root.Element("User") ?? throw LoadFailedException("User");
            user = new(userElement);

            XElement contactsElement = doc.Root.Element("Contacts") ?? throw LoadFailedException("Contacts");
            contacts = contactsElement.Elements("User")
                                      .Select(user => new User(user))
                                      .ToList();

            XElement chatsElement = doc.Root.Element("Chats") ?? throw LoadFailedException("Chats");
            chats = chatsElement.Elements("Chat")
                                .Select(chat => new Chat(chat, contacts.Prepend(user).ToList()))
                                .ToList();

            Messenger messenger = new(user);

            contacts.ForEach(messenger.NewContact);
            chats.ForEach(messenger.AddChat);

            return new(user);
        }

        public static void RenameOldFile()
        {
            if (SaveFile.Exists) throw new InvalidOperationException("Файл сохранения уже существует");

            foreach (var file in OldSaveFiles)
            {
                if (file.Exists)
                {
                    File.Copy(file.FullName, SaveFile.FullName);
                    File.Delete(file.FullName);
                    return;
                }
            }

            throw new InvalidOperationException("Нет файлов для восстановления");
        }
    }
}

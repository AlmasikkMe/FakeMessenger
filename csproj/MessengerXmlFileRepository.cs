using System.Xml.Linq;

namespace FakeMessenger;
public class MessengerXmlFileRepository
{
    public List<FileInfo> OldSaveFiles { get; set; } = [new("Save.Messager.xml")];
    public FileInfo SaveFile { get; set; } = new("Messenger.Save.xml");
    public void Save(Messenger messenger) => MessengerXmlSerializer.Serialize(messenger).Save(SaveFile.FullName);
    public Messenger Load() => MessengerXmlSerializer.DeserializeMessenger(XDocument.Load(SaveFile.FullName), [SaveFile.FullName]);
    public void RenameOldFile()
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

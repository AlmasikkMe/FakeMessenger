using FakeMessenger.Core;
using System.IO;
using System.Xml.Linq;

namespace FakeMessenger.FileRepository.Xml;

public class XmlFileRepository(XmlSerializer serializer) : Repository
{
    private XmlSerializer Serializer { get; set; } = serializer;
    public List<FileInfo> OldSaveFiles { get; set; } = [new("Save.Messager.xml")];
    override public FileInfo SaveFile { get; set; } = new("Messenger.Save.xml");
    override public void Save(Messenger messenger) => Serializer.SerializeMessenger(messenger).Save(SaveFile.FullName);
    override public Messenger Load()
    {
        if (!SaveFile.Exists) RenameOldFile();

        return Serializer.DeserializeMessenger(XDocument.Load(SaveFile.FullName), this, [SaveFile.FullName]);
    }
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

using FakeMessenger.Core;
using System.IO;

namespace FakeMessenger.FileRepository
{
    abstract public class FileRepository
    {
        abstract public FileInfo SaveFile { get; set; }
        abstract public void Save(Messenger messenger);
        abstract public Messenger Load();
    }
}

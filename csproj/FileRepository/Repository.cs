using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using FakeMessenger.Core;

namespace FakeMessenger.FileRepository
{
    abstract public class Repository
    {
        abstract public FileInfo SaveFile { get; set; }
        abstract public void Save(Messenger messenger);
        abstract public Messenger Load();
    }
}

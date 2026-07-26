using System.Xml.Linq;

namespace FakeMessenger;

public class Message(User sender)
{
    public User Sender = sender;
    public string Text = "";
    public string Type = "text";
    public DateTime DateTime = DateTime.Now;
}

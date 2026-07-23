using System.Xml.Linq;

namespace FakeMessenger;

public class Message(User sender)
{
    public User Sender = sender;
    public string Text = "";
    public string Type = "text";
    public DateTime DateTime = DateTime.Now;

    public XElement ToXElement() =>
        new("Message",
            new XAttribute("Sender", Sender.Username),
            new XAttribute("Type", Type),
            new XAttribute("DateTime", DateTime),
            Text);

}

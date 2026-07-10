using System.Xml.Linq;

namespace ConsoleFakeChat;

public class Message(User sender)
{
    public User Sender = sender;
    public string Text = "";
    public string Type = "text";
    public DateTime DateTime = DateTime.Now;

    public XElement ToXElement() =>
        new("Message",
            new XElement("Sender", Sender.Username),
            new XElement("Text", Text),
            new XElement("Type", Type),
            new XElement("DateTime", DateTime)
            );

}

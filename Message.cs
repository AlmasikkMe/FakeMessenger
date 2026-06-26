namespace ConsoleFakeChat;

public class Message(User sender)
{
    public Message() : this(new()) { }

    public User Sender = sender;
    public string Text = "";
    public string Type = "text";
    public DateTime DateTime = DateTime.Now;
}

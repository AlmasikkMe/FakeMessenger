namespace FakeMessenger.Core;

public class Message(User sender)
{
    public User Sender { get; set; } = sender;
    public string Text { get; set; } = "";
    public string Type { get; set; } = "text";
    public DateTime DateTime { get; set; } = DateTime.Now;
}

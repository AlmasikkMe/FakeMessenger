public class Message(string sender, string text, string type = "text")
{
    public string Sender = sender;
    public string Text = text;
    public string Type = type;
    public DateTime DateTime = DateTime.Now;
}

namespace ConsoleFakeChat;

public class User(string username, string firstName, string lastName = "")
{
    public User() : this("@example", "Example contact") { }

    public string Username
    {
        get;
        set {
            string username = value;
            if (!username.StartsWith("@")) username = $"@{username}";

            if (username.Skip(1).All(c => char.IsAsciiLetter(c) || char.IsNumber(c)))
            {
                if (username.Length > 5) field = username;
                else throw new ArgumentException("Имя пользователя должно содержать 5 или более символов (не считая @)");
            }
            else throw new ArgumentException("Имя пользователя допускает только латинские символы или цифры");
        }
    } = username;

    public string FirstName 
    { 
        get;
        set => field = !value.IsWhiteSpace() ? value.Trim() : throw new ArgumentException("Имя не может быть пустым"); 
    } = firstName;
    public string LastName 
    {
        get;
        set => field = value.Trim(); 
    } = lastName;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public Chat Chat 
    { 
        get 
        { 
            if (field == null)
            { 
                field = new(FullName);
                Chat.AddMembers([Messenger.User, this]);
            } 
            field.ChatName = FullName; return field; 
        } 
    }
    private Chat? _chat = null;
    public bool IsHasChat => !(_chat == null);
}

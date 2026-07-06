namespace ConsoleFakeChat;

public class User
{
    public User(string username, string firstName, string lastName = "") 
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
    }
    public User() : this("@example", "Пользователь") { }

    public string Username
    {
        get;
        set {
            string username = value;
            if (!username.StartsWith("@")) username = $"@{username}";

            if (username.Skip(1).All(char.IsAsciiLetterOrDigit))
            {
                if (username.Length - 1 is >= 5 and <= 20) field = username;
                else throw new ArgumentException("Имя пользователя должно содержать от 5 до 20 символов (не считая @)");
            }
            else throw new ArgumentException("Имя пользователя допускает только латинские символы или цифры");
        }
    }

    public string FirstName 
    { 
        get;
        set => field = !value.IsWhiteSpace() ? value.Trim() : throw new ArgumentException("Имя не может быть пустым"); 
    } 
    public string LastName 
    {
        get;
        set => field = value.Trim(); 
    } 
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

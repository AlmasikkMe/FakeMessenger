using System.Xml.Linq;

namespace FakeMessenger;

public class User
{
    public User(string username, string firstName, string lastName = "") 
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
    }    
    public User(XElement xElement)
    {
        Username = (xElement.Element("Username") ?? throw new InvalidOperationException("Обязательный элемент Username не найден в элементе")).Value;
        FirstName = (xElement.Element("FirstName") ?? throw new InvalidOperationException("Обязательный элемент FirstName не найден в элементе")).Value;
        XElement? lastNameElement = xElement.Element("LastName");
        LastName = lastNameElement is null ? "" : lastNameElement.Value;
    }

    public string Username
    {
        get;
        set {
            string username = value.Trim();
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
            if (_chat == null)
            { 
                _chat = new(Username, FullName);
                Chat.AddMembers([Messenger.User, this]);
            } 
            _chat.Name = FullName; return _chat; 
        }
        set
        {
            if (value.ChatName == Username) _chat = value;
            else throw new ArgumentException("Уникальное имя чата должно соответствовать имени пользователя");
        }
    }
    private Chat? _chat = null;
    public bool IsHasChat => !(_chat == null);

    public XElement ToXElement() =>
        new ("User",
            new XElement("Username", Username),
            new XElement("FirstName", FirstName),
            new XElement("LastName", LastName),
            new XElement("IsHasChat", IsHasChat)
            );
}

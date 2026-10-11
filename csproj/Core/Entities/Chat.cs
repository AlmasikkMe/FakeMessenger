using System.Data;
using FakeMessenger.Core.Repositories;

namespace FakeMessenger.Core.Entities;

public class Chat
{
    public string ChatName
    {
        get;
        set
        {
            string chatName = value.Trim();
            if (!chatName.StartsWith("@")) chatName = $"@{chatName}";

            if (chatName.Skip(1).All(char.IsAsciiLetterOrDigit))
            {
                if (chatName.Length - 1 is >= 5 and <= 20) field = chatName;
                else throw new ArgumentException("Уникальное имя чата должно содержать от 5 до 20 символов (не считая @)");
            }
            else throw new ArgumentException("Уникальное имя чата допускает только латинские символы или цифры");
        }
    }
    public string Name
    {
        get;
        set => field = !value.IsWhiteSpace() ? value.Trim() : throw new ArgumentException("Название чата не может быть пустым");
    }

    public UsersRepository MembersRepository = new();
    public MessagesRepository MessagesRepository = new();

    [Obsolete("Используйте MembersRepository.Get()")]
    public IReadOnlyList<User> Members => MembersRepository.Get();

    [Obsolete("Используйте MessagesRepository.Get()")]
    public IReadOnlyList<Message> Messages => MessagesRepository.Get();

    public Chat(string chatName, string name)
    {
        ChatName = chatName;
        Name = name;
    }

    [Obsolete("Используйте MembersRepository.Get()")]
    public List<User> GetMembers(string search = "")
    {
        return (from member in Members
                where member.Username.Contains(search, StringComparison.OrdinalIgnoreCase)
                select member)
                .ToList();
    }

    [Obsolete("Используйте MembersRepository.Add()")]
    public void AddMembers(List<User> members)
    {
        foreach (User member in members) MembersRepository.Add(member);
    }

    [Obsolete("Используйте MessagesRepository.Add()")]
    public void AddMessage(Message message)
    {
        MessagesRepository.Add(message);
    }

    [Obsolete("Используйте MessagesRepository.Add()")]
    public void AddMessage(User sender, string? text = null, string? type = null, DateTime? dateTime = null)
    {
        Message message = new(sender);

        if (text != null) message.Text = text;
        if (type != null) message.Type = type;
        if (dateTime != null) message.DateTime = (DateTime)dateTime;

        MessagesRepository.Add(message);
    }

    [Obsolete("Используйте MessagesRepository.Remove()")]
    public void DeleteMessage(Message message)
    {
        MessagesRepository.Remove(message);
    }
}

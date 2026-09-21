namespace FakeMessenger.UI.WpfUI.Models;

public sealed record ChatItem(
    string Title,
    string LastMessage,
    string Time,
    string AvatarText,
    int UnreadCount)
{
    public bool HasUnread => UnreadCount > 0;
}
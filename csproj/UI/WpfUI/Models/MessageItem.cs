namespace FakeMessenger.UI.WpfUI.Models;

public sealed record MessageItem(
    string Text,
    string Time,
    bool IsOutgoing);   
public class Handler
{
    public static Dictionary<string, (string Emoji, string Name, bool IsWithTime)> MessagesTypes = new()
    {
        { "photo",        ("🖼", "Фотография",          false) },
        { "video",        ("📹", "Видеозапись",         false) },
        { "video_note",   ("📹", "Видеосообщение",      true)  },
        { "gif",          ("💥", "GIF",                 false) },
        { "voice",        ("🎤", "Голосовое сообщение", true)  },
        { "audio",        ("🎶", "Аудиозапись",         false) },
        { "voice_effect", ("🔊", "Голосовой эффект",    false) },
        { "file",         ("📂", "Файл",                false) },
        { "sticker",      ("🎨", "Стикер",              false) },
        { "emoji",        ("🎨", "Эмодзи",              false) },
        { "poll",         ("📊", "Опрос",               false) },
        { "quiz",         ("📊", "Викторина",           false) },
        { "contact",      ("👤", "Контакт",             false) },
        { "location",     ("📍", "Геолокация",          false) },
        { "live_location",("🚨", "Живая геолокация",    false) },
        { "game",         ("🎮", "Игра",                false) },
        { "product",      ("🛍", "Продукт",             false) },
        { "gift",         ("🎁", "Подарок",             false) }
    };
}

public class Message(string sender, string text, string type = "text")
{
    public string Sender = sender;
    public string Text = text;
    public string Type = type;
    public DateTime DateTime;
}

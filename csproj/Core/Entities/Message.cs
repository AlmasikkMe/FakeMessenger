using System.ComponentModel;

namespace FakeMessenger.Core.Entities;

public class Message(User sender) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public User Sender 
    { 
        get; 
        set 
        {
            field = value;
            PropertyChanged?.Invoke(this, new(nameof(Sender))); 
        }
    } = sender;
    public string Text 
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new(nameof(Text)));
        }
    } = "";
    public string Type
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new(nameof(Type)));
        }
    } = "text";
    public DateTime DateTime 
    { 
        get; 
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new(nameof(DateTime)));
        } 
    } = DateTime.Now;
}
namespace SmsMachine.Services;

public interface INotifyService
{
    void Notify(string index, string recipient, string text, string date);
}

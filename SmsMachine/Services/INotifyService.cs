namespace SmsMachine.Services;

public interface INotifyService
{
    void Notify(string recipient, string text, string date, int indexSms, string status);
}

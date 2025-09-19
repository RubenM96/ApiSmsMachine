
namespace SmsMachine.Services
{
    public interface ISmsSender
    {
        int SendSms(string recipient, string text, bool notify);
    }
}

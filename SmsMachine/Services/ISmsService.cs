namespace SmsMachine.Services
{
    public interface ISmsService
    {
        void SendSms(string recipient, string text, bool multipart, bool notify);
    }
}
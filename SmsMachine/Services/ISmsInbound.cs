
namespace SmsMachine.Services
{
    public interface ISmsInbound
    {
        void SmsInbound(string recipient, string text, string date);

    }
}

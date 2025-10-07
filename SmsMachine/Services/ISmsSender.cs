
using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public interface ISmsSender
    {
        AreaSxSendResult SendSms(string recipient, string text, bool notify);
    }
}

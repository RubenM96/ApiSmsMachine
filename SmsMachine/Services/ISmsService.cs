using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public interface ISmsService
    {
        AreaSxSendResult SendSms(string recipient, string text, bool multipart, bool notify, int? campaignId);
    }
}
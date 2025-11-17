using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public interface ISmsService
    {
        AreaSxSendResult SendSms(int? smsId, string recipient, string text, bool multipart, bool notify, int? campaignId);
    }
}
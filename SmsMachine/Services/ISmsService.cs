using SmsMachine.Infrastructure;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ISmsService
    {
        AreaSxSendResult SendSms(SmsOutbound smsOutbound);
    }
}
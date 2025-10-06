using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsOutboundRepository
    {
        SmsOutbound AddSms(SmsOutbound sms);
        SmsOutbound? GetSms(int id);
        SmsOutbound? GetSmsOutboundByRecipientAndIndex(string recipient, int indexSms);
    }
}

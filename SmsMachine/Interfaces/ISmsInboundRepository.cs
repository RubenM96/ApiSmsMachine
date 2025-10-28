using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsInboundRepository
    {
        SmsInbound AddSmsInbound(SmsInbound smsInbound);
        SmsInbound? GetSmsInbound(int id);

    }
}

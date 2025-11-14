using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsOutboundRepository
    {
        SmsOutbound AddSms(SmsOutbound sms);
        SmsOutbound? GetSmsById(int id);
        List<SmsOutbound> GetAllSmsByCampaignId(int campaignId);
        SmsOutbound? GetSmsOutboundByRecipientAndIndex(string recipient, int indexSms);
        SmsOutbound? GetSmsOutboundByIndexAndCampaignId(int indexSms, int campaignId);
    }
}

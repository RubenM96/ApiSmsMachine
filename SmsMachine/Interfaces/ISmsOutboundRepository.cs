using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsOutboundRepository
    {
        SmsOutbound AddSms(SmsOutbound sms);
        SmsOutbound? GetSmsById(int id);
        void UpdateSms(SmsOutbound sms);
        List<SmsOutbound> GetAllSmsByCampaignId(int campaignId);
        void DeleteAllSmsByCampaignId(int campaignId);
        SmsOutbound? GetSmsOutboundByRecipientAndIndex(string recipient, int indexSms);
        SmsOutbound? GetSmsOutboundByIndexAndCampaignId(int indexSms, int campaignId);

        SmsOutbound? GetSmsOutboundInProgress();
    }
}

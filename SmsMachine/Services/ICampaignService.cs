using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        //scheduledAt non è ancora implementato
        CampaignSms CreateCampaign(string title, string text, string recipientList, bool campaignNotify, string? description);

        CampaignSms SendCampaign(int id);

        CampaignSms GetCampaign(int id);

    }
}
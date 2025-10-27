using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        //scheduledAt non è ancora implementato
        CampaignSms CreateCampaign(string title, string text, string recipientList, bool campaignNotify, string? description);

        Task<CampaignSms> SendCampaign(int id);

        IEnumerable<CampaignSms> GetAllCampaigns();

        CampaignSms? GetCampaignId(int id);

        CampaignSms UpdateCampaign(int id, string title, string text, string recipientList, bool campaignNotify, string? description);

        bool DeleteCampaign(int id);

        public Task RetryQueuedForCampaignAsync(int campaignId, TimeSpan delay);
    }
}
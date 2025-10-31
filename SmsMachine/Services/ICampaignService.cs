using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        CampaignSms CreateCampaign(string title, string text, string recipientList, bool campaignNotify, string? description);

        Task<CampaignSms> SendCampaign(int id);

        CampaignSms UpdateCampaign(int id, string title, string text, string recipientList, bool campaignNotify, string? description);
        
        public Task RetryQueuedForCampaignAsync(int campaignId, TimeSpan delay);
    }
}
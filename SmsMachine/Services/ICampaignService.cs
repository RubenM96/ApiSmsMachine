using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        Task CreateCampaignAsync(CreateCampaignRequest campaignRequest);

        Task SendCampaign(int id);

        Task<CampaignSms> UpdateCampaign(int id, CreateCampaignRequest form);

        public Task DeleteCampaignById(int id);
    }
}
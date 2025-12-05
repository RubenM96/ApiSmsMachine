using SmsMachine.Dashboard.Models;

namespace SmsMachine.Dashboard.Services
{
    public interface ICampaignService
    {
        Task<CampaignDetails?> GetCampaignIdAsync(int id);
        Task<HttpResponseMessage> SendCampaignAsync(int id);

        Task<HttpResponseMessage> CreateCampaignAsync(CampaignForm campaignForm);
        Task<HttpResponseMessage> UpdateCampaignAsync(int id, CampaignForm campaignForm);
        Task<HttpResponseMessage> DeleteCampaignAsync(int id);

        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignSearchQuery campaignSearchQuery);

        Task<CampaignProgressDTO?> GetProgressAsync(int campaignId);
        Task<string> GetRecipientsByCampaignIdAsync(int campaignId);
    }
}

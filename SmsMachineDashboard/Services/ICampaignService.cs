using SmsMachine.Dashboard.Models;

namespace SmsMachine.Dashboard.Services
{
    public interface ICampaignService
    {
        Task<CampaignFormResult> CreateCampaignAsync(CampaignForm campaignForm);
        Task<bool> DeleteCampaignAsync(int id);
        Task<CampaignDetails?> GetCampaignIdAsync(int id);
        Task<string> GetRecipientsByCampaignIdAsync(int campaignId);
        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignSearchQuery campaignSearchQuery);
        Task<bool> SendCampaignAsync(int id);
        Task<CampaignFormResult> UpdateCampaignAsync(int id, CampaignForm campaignForm);
        Task<CampaignProgressDTO?> GetProgressAsync(int campaignId);
    }
}

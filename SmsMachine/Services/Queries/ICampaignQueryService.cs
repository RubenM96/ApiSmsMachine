using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;

namespace SmsMachine.Api.Services.Queries
{
    public interface ICampaignQueryService
    {
        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter);
        Task<CampaignProgressDTO> GetCampaignProgress(int campaignId);
        Task<CampaignSummaryDTO> GetSummaryAsync();
    }
}

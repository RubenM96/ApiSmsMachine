using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ICampaignRepository
    {
        CampaignSms AddCampaign(CampaignSms campaign);
        IEnumerable<CampaignSms> GetAllCampaigns();
        IEnumerable<CampaignListDTO> GetAllCampaignsViews();
        CampaignSms? GetCampaignId(int id);
        CampaignSms UpdateCampaign(CampaignSms campaign);
        bool DeleteCampaign(int id);
        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter);
    }
}

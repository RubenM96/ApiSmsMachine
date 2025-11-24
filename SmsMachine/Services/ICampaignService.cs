using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        CampaignSms CreateCampaign(CampaignSms campaign, string recipientList);

        Task<CampaignSms> SendCampaign(int id);

        CampaignSms UpdateCampaign(int id, CampaignForm form);

        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter);
    }
}
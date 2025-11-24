using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        CampaignSms CreateCampaign(CampaignSms campaign, string recipientList);

        Task<CampaignSms> SendCampaign(int id);

        //CampaignSms UpdateCampaign(int id, string title, string text, string recipientList, bool campaignNotify, string? description);

        public bool DeleteCampaignById(int id);

        Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter);
    }
}
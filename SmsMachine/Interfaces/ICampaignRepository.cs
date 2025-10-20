using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ICampaignRepository
    {
        CampaignSms AddCampaign(CampaignSms campaign);
        IEnumerable<CampaignSms> GetAllCampaigns();
        CampaignSms? GetCampaignId(int id);
        CampaignSms UpdateCampaign(CampaignSms campaign);

    }
}

using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ICampaignRepository
    {
        CampaignSms AddCampaign(CampaignSms campaign);
        CampaignSms GetCampaignId(int id);
        CampaignSms UpdateCampaignStatus (CampaignSms campaign);

    }
}

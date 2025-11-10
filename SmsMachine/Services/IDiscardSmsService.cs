using SmsMachine.Models;

namespace SmsMachine.Api.Services
{
    public interface IDiscardSmsService
    {
        List<SmsOutbound> RecoveryDiscardedSmsByCampaignId(int campaignId);
    }
}

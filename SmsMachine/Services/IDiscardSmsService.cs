using SmsMachine.Models;

namespace SmsMachine.Api.Services
{
    public interface IDiscardSmsService
    {
        Task<List<SmsOutbound>> RecoveryDiscardedSmsByCampaignId(int campaignId);
    }
}

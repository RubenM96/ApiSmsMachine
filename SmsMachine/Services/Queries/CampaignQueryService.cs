using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Interfaces;
using SmsMachine.Api.Infrastructure.Utils;

namespace SmsMachine.Api.Services.Queries
{
    public class CampaignQueryService : ICampaignQueryService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsOutboundRepository _smsOutboundRepository;

        public CampaignQueryService(
            ICampaignRepository campaignRepository,
            ISmsOutboundRepository smsOutboundRepository)
        {
            _campaignRepository = campaignRepository;
            _smsOutboundRepository = smsOutboundRepository;
        }
        public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter)
        {
            filter.Normalize();                      // normalizza page/pageSize e From/To
            return await _campaignRepository.SearchAsync(filter);
        }

        public CampaignProgressDTO GetCampaignProgress(int campaignId)
        {
            var campaign = _campaignRepository.GetCampaignId(campaignId);
            if (campaign == null) 
            {
                throw new ArgumentException($"Campaign with ID {campaignId} not found");
            } 
                
            var sms = _smsOutboundRepository.GetAllSmsByCampaignId(campaignId).ToList();

            
            return new CampaignProgressDTO
            {
                CampaignId = campaign.Id,
                Status = campaign.Status,
                Total = sms.Count,
                Sent = sms.Count(s => s.Status == SmsStatus.Sent),
                Discard = sms.Count(s => s.Status == SmsStatus.Discard),
                Failed = sms.Count(s => s.Status == SmsStatus.Failed),
                InProgress = sms.Count(s => s.Status == SmsStatus.InProgress),
                Draft = sms.Count(s => s.Status == SmsStatus.Draft)
            };
        }
    }
}

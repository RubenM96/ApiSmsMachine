using SmsMachine.Interfaces;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Api.Services
{
    public class DiscardSmsService : IDiscardSmsService
    {

        private readonly ISmsDiscard _smsDiscard;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly ILogger<DiscardSmsService> _logger;

        public DiscardSmsService(ISmsDiscard smsDiscard, ISmsOutboundRepository smsOutboundRepository, ILogger<DiscardSmsService> logger)
        {
            _smsDiscard = smsDiscard;
            _smsOutboundRepository = smsOutboundRepository;
            _logger = logger;
        }

        public List<SmsOutbound> RecoveryDiscardedSmsByCampaignId(int campaignId)
        {
            try
            {
                var indexSmsList = _smsDiscard.SmsNotSend().SmsTxErrIdx.Split('.').Select(int.Parse).ToList();
                var recoveredSmsList = new List<SmsOutbound>();

                foreach (var indexSms in indexSmsList)
                {
                    var sms = _smsOutboundRepository.GetSmsOutboundByIndexAndCampaignId(indexSms, campaignId);
                    if (sms != null)
                    {
                        recoveredSmsList.Add(sms);
                    }
                }

                return recoveredSmsList;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while recovering discarded SMS for CampaignId.");
                throw;
            }
        }


    }
}

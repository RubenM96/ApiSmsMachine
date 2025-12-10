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

        /// <summary>
        /// Recupera gli SMS scartati per la campagna specificata.
        /// </summary>
        /// <remarks>Questo metodo recupera gli indici dei messaggi scartati da AreaSx 
        /// per fare una ricerca dei Sms attraverso gli indici dei messaggi e l'id della campagna.
        /// </remarks>
        /// <param name="campaignId">L'identificativo univoco della campagna per la quale devono essere recuperati i messaggi SMS scartati.</param>
        /// <returns>Una lista di messaggi SMS in uscita che sono stati scartati per la campagna indicata. La lista sarà vuota se non vengono trovati messaggi scartati.</returns>

        public async Task<List<SmsOutbound>> RecoveryDiscardedSmsByCampaignId(int campaignId)
        {
            try
            {
                var indexSmsList = _smsDiscard.SmsNotSend().SmsTxErrIdx.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
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

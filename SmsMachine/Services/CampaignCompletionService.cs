using SmsMachine.Api.Infrastructure.Utils;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Api.Services
{
    public class CampaignCompletionService : ICampaignCompletionService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly IDiscardSmsService _discardSmsService;
        private readonly ILogger<CampaignCompletionService> _logger;

        public CampaignCompletionService(
            ICampaignRepository campaignRepository,
            ISmsOutboundRepository smsOutboundRepository,
            IDiscardSmsService discardSmsService,
            ILogger<CampaignCompletionService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsOutboundRepository = smsOutboundRepository;
            _discardSmsService = discardSmsService;
            _logger = logger;
        }


        /// <summary>
        /// Verifica lo stato di completamento di tutte le campagne attualmente in corso
        /// e aggiorna il loro stato quando necessario.
        /// </summary>
        /// <remarks>
        /// Cerca tutte le campagne con stato "InProgress" e controlla i singoli SMS associati per singola campagna.
        /// </remarks>

        public Task CheckCampaignsCompletionAsync()
        {
            _logger.LogInformation("Verifica campagne in stato InProgress...");

            // prendo tutte le campagne ancora in corso
            var campaigns = _campaignRepository.GetCampaignsInProgress();

            foreach (var campaign in campaigns)
            {
                try
                {
                    CheckSingleCampaignCompletion(campaign);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Errore durante il controllo della campagna {CampaignId}",
                        campaign.Id);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Verifica se la campagna SMS specificata è stata completata in base allo stato
        /// dei messaggi ad essa associati e aggiorna il suo stato se tutti i messaggi
        /// si trovano in uno stato terminale.
        /// </summary>
        /// <remarks>
        /// Una campagna è considerata completata quando tutti i messaggi SMS associati
        /// si trovano in uno stato terminale (come Inviato, Scartato o Fallito).
        /// Se la campagna risulta completata, il suo stato viene aggiornato di conseguenza.
        /// </remarks>
        /// <param name="campaign">
        /// La campagna da verificare per il completamento.
        /// </param>

        private async Task CheckSingleCampaignCompletion(CampaignSms campaign)
        {
            // prendo tutti gli SMS della campagna
            var smsList = await _smsOutboundRepository.GetAllSmsByCampaignId(campaign.Id);

            //check dei messaggi scartati
            //await CheckDiscardSmsForSingleCampaign(campaign);           

            if (!campaign.IsComplete(smsList))
            {
                // nessun SMS
                _logger.LogWarning(
                    "La campagna {CampaignId} è InProgress ma non ha SMS associati",
                    campaign.Id);
                return;
            }

            // stato terminale
            bool IsTerminal(SmsStatus status) =>
                status == SmsStatus.Sent ||
                status == SmsStatus.Discard ||
                status == SmsStatus.Failed;

            var allTerminal = smsList.All(s => IsTerminal(s.Status));

            if (!allTerminal)
            {
                // ci sono ancora SMS non terminali (Draft/InProgress)
                _logger.LogInformation(
                    "Campagna {CampaignId} non ancora completa: esistono SMS non terminali",
                    campaign.Id);
                return;
            }

            // tutti terminali = campagna finita
            campaign.MarkFinished();
            _campaignRepository.UpdateCampaign(campaign);

            _logger.LogInformation(
                "Campagna {CampaignId} marcata come Finished (tutti gli SMS in stato terminale)",
                campaign.Id);
        }

        /// <summary>
        /// Segna tutti i messaggi SMS associati alla campagna specificata come scartati e aggiorna il loro stato nel
        /// repository.
        /// </summary>
        /// <remarks>Questo metodo recupera tutti i messaggi SMS scartati per la campagna indicata
        /// e li segna nuovamente come scartati.
        /// </remarks>
        /// <param name="campaign">La campagna i cui messaggi SMS associati saranno contrassegnati come scartati.
        /// </param>

        public async Task CheckDiscardSmsForSingleCampaign(CampaignSms campaign)
        {
            var discardedSmsList = await _discardSmsService.RecoveryDiscardedSmsByCampaignId(campaign.Id);

            foreach (var sms in discardedSmsList)
            {
                sms.MarkDiscarded();
                _smsOutboundRepository.UpdateSms(sms);

                _logger.LogInformation("SMS {SmsId} della campagna {CampaignId} marcato come Discard",sms.Id, campaign.Id);
            }
        }

    }
}

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

        private async Task CheckSingleCampaignCompletion(CampaignSms campaign)
        {
            // prendo tutti gli SMS della campagna
            var smsList = await _smsOutboundRepository.GetAllSmsByCampaignId(campaign.Id);

            //check dei messaggi scartati
            await CheckDiscardSmsForSingleCampaign(campaign);           

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

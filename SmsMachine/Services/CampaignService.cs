using SmsMachine.Api.Infrastructure.Utils;
using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Api.Services;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly IDiscardSmsService _discardSmsService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ISmsOutboundRepository smsOutboundRepository,
            IDiscardSmsService discardSmsService,
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsOutboundRepository = smsOutboundRepository;
            _discardSmsService = discardSmsService;
            _logger = logger;
        }

        //Creazione campagna
        public CampaignSms CreateCampaign(CampaignSms campaign, string recipientList)
        {

            RecipientList recipients = new RecipientList(recipientList);

            var createdCampaign = _campaignRepository.AddCampaign(campaign);

            //crea i singoli sms della campagna
            foreach (var recipient in recipients.GetRecipientToList(recipientList))
            {
                var sms = new SmsOutbound(new Recipient(recipient), campaign.Text, false, campaign.CampaignNotify, DateTime.Now, createdCampaign.Id);
                try
                {
                    _smsOutboundRepository.AddSms(sms);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create SMS for recipient {Recipient} in Campaign ID {CampaignId}", recipient, createdCampaign.Id);
                }
            }

            _logger.LogInformation("Created campaign with ID {CampaignId}", createdCampaign.Id);
            return createdCampaign;
        }

        // Invio Campagna
        public async Task<CampaignSms> SendCampaign(int id)
        {
            CampaignSms? campaign = _campaignRepository.GetCampaignId(id);

            if (campaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            campaign.Status = CampaignStatus.InProgress;
            _campaignRepository.UpdateCampaign(campaign);

            var smsList = _smsOutboundRepository.GetAllSmsByCampaignId(id);

            foreach (var smsOutbound in smsList)
            {
                try
                {
                    smsOutbound.Status = SmsStatus.InProgress;
                    _smsOutboundRepository.UpdateSms(smsOutbound);
                    _logger.LogInformation("SMS to {Recipient} for Campaign ID {CampaignId} set in progress", smsOutbound.Recipient.Value, campaign.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to set SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                }
            }

            return campaign;
        }

        /*
        // Modifica Campagna
        public CampaignSms UpdateCampaign(int id, string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            CampaignSms? existingCampaign = _campaignRepository.GetCampaignId(id);
            if (existingCampaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found for update", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            
            var recipients = existingCampaign.GetRecipientToList(recipientList);

            _smsOutboundRepository.DeleteAllSmsByCampaignId(existingCampaign.Id);
            
            foreach (var recipient in recipients)
            {
                var sms = new SmsOutbound(new Recipient(recipient), text, false, campaignNotify, DateTime.Now, existingCampaign.Id);
                try
                {
                    _smsOutboundRepository.AddSms(sms);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create SMS for recipient {Recipient} in Campaign ID {CampaignId} during update", recipient, existingCampaign.Id);
                }
            }

            existingCampaign.Title = title;
            existingCampaign.Text = text;
            existingCampaign.CampaignNotify = campaignNotify;
            existingCampaign.Description = description;
            existingCampaign.TotalRecipients = existingCampaign.CalculateTotalRecipients(recipients);

            var updatedCampaign = _campaignRepository.UpdateCampaign(existingCampaign);

            _logger.LogInformation("Updated campaign with ID {CampaignId}", updatedCampaign.Id);
            return updatedCampaign;
        }
        */

        public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter)
        {
            filter.Normalize();                      // normalizza page/pageSize e From/To
            return await _campaignRepository.SearchAsync(filter);
        }
        
        public bool DeleteCampaignById(int id)
        {
            var ok = _campaignRepository.DeleteCampaign(id);
            if (ok)
            {
                _logger.LogInformation("Deleted campaign with ID {CampaignId}", id);
                //per l'integrità referenziale elimina anche tutti gli sms associati
                _smsOutboundRepository.DeleteAllSmsByCampaignId(id);
                _logger.LogInformation("Deleted all SMS for Campaign ID {CampaignId}", id);
            }
            else {
                _logger.LogWarning("Failed to delete campaign with ID {CampaignId}", id);
            }

            return ok;
        }


        //metodo per calcolare lo stato della campagna in base ai messaggi inviati/falliti
        public bool CampaignIsComplete(CampaignSms campaign)
        {
            CountDiscardedSmsByCampaignId(campaign);
            CountDeliveredSmsByCampaignId(campaign);

            return campaign.IsComplete();
        }

        //metodo per contare i messaggi scartati
        public int CountDiscardedSmsByCampaignId(CampaignSms campaign)
        {
            var failedSmsCount = _discardSmsService.RecoveryDiscardedSmsByCampaignId(campaign.Id).Count();
            _logger.LogInformation("Counted {FailedSmsCount} discarded SMS for Campaign ID {CampaignId}", failedSmsCount, campaign.Id);

            campaign.IncFailed(failedSmsCount);
            return failedSmsCount;
        }

        //metodo per contare i messaggi inviati 
        public int CountDeliveredSmsByCampaignId(CampaignSms campaign)
        {
            var totalSent = _smsOutboundRepository.GetAllSmsByCampaignId(campaign.Id).Count();
            _logger.LogInformation("Counted {TotalSent} delivered SMS for Campaign ID {CampaignId}", totalSent, campaign.Id);

            campaign.IncDelivered(totalSent);
            return totalSent;
        }


    }
}

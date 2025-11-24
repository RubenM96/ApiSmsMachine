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
        private readonly ISmsQueueRepository _smsQueueRepository;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly IDiscardSmsService _discardSmsService;
        private readonly ISmsService _smsService;
        private readonly ISmsQueueService _smsQueueService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ISmsQueueRepository smsQueueRepository,
            ISmsOutboundRepository smsOutboundRepository,
            IDiscardSmsService discardSmsService,
            ISmsService msService,
            ISmsQueueService smsQueueService,
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsQueueRepository = smsQueueRepository;
            _smsOutboundRepository = smsOutboundRepository;
            _discardSmsService = discardSmsService;
            _smsService = msService;
            _smsQueueService = smsQueueService;
            _logger = logger;
        }

        //Creazione campagna
        public CampaignSms CreateCampaign(CampaignSms campaign, string recipientList)
        {
            //CampaignSms campaign = new CampaignSms(title, text, recipientList, campaignNotify, description);
            
            var createdCampaign = _campaignRepository.AddCampaign(campaign);

            //crea i singoli sms della campagna
            foreach (var recipient in campaign.GetRecipientToList(recipientList))
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
                    _smsService.SendSms(smsOutbound.Id, smsOutbound.Recipient.Value, smsOutbound.Text, smsOutbound.Multipart, smsOutbound.Notify, campaign.Id);
                    _logger.LogInformation("Sent SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                }
            }

            //Tentativo di invio dei messaggi in coda
            // await RetryQueuedForCampaignAsync(campaign.Id, TimeSpan.FromSeconds(5));
            await _smsQueueService.ProcessSmsQueueAsync(TimeSpan.FromSeconds(5), campaign);


            //controllo messaggi consegnati/falliti e cambio stato campagna
            var isComplete = CampaignIsComplete(campaign);
            if (isComplete)
            {
                campaign.Status = CampaignStatus.Finished;
                _campaignRepository.UpdateCampaign(campaign);
            }

            return campaign;
        }

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

        //tentativo di invio dei messaggi in coda per una campagna specifica
        public async Task RetryQueuedForCampaignAsync(int campaignId, TimeSpan delay)
        {
            try
            {
                do
                {
                    await Task.Delay(delay);

                    var queued = _smsQueueRepository.GetSmsQueueByCampaign(campaignId);

                    foreach (var smsQueue in queued)
                    {
                        try
                        {
                            _smsService.SendSms(null, smsQueue.Recipient.Value, smsQueue.Text, smsQueue.Multipart, smsQueue.Notify, campaignId);

                            //rimuovi sms dalla coda
                            _smsQueueRepository.Delete(smsQueue.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Retry failed for Campaign {CampaignId}, Recipient {Recipient}",
                                campaignId, smsQueue.Recipient.Value);
                        }
                    }
                } while (_smsQueueRepository.GetSmsQueueByCampaign(campaignId).Count > 0);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in retry sms queue for Campaign {CampaignId}", campaignId);
            }
        }

        public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter)
        {
            filter.Normalize();                      // normalizza page/pageSize e From/To
            return await _campaignRepository.SearchAsync(filter);
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

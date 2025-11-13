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
        private readonly IDiscardSmsService _discardSmsService;
        private readonly ISmsService _smsService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ISmsQueueRepository smsQueueRepository,
            IDiscardSmsService discardSmsService,
            ISmsService msService,
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsQueueRepository = smsQueueRepository;
            _discardSmsService = discardSmsService;
            _smsService = msService;
            _logger = logger;
        }

        //Creazione campagna
        public CampaignSms CreateCampaign(string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            CampaignSms campaign = new CampaignSms(title, text, recipientList, campaignNotify, description);

            var createdCampaign = _campaignRepository.AddCampaign(campaign);
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

            //foreach (var recipient in campaign.RecipientList.Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()))
            foreach (var recipient in campaign.GetRecipientToList(campaign.RecipientList))
            {
                try
                {
                    _smsService.SendSms(recipient, campaign.Text, false, campaign.CampaignNotify, campaign.Id);
                    _logger.LogInformation("Sent SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
            }

            //Tentativi di invio della coda
            await RetryQueuedForCampaignAsync(campaign.Id, TimeSpan.FromSeconds(5)); // tentativo di invio dei messaggi in coda 


            //controllo messaggi consegnati/ falliti e cambio stato campagna


            campaign.Status = CampaignStatus.Finished;
            _campaignRepository.UpdateCampaign(campaign);
            
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

            existingCampaign.Title = title;
            existingCampaign.Text = text;
            existingCampaign.RecipientList = existingCampaign.RegrexRecipient(recipients);
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
                            _smsService.SendSms(smsQueue.Recipient.Value, smsQueue.Text, smsQueue.Multipart, smsQueue.Notify, campaignId);

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

        //metodo per contare i messaggi scartati
        public int CountDiscardedSmsByCampaignId(CampaignSms campaign)
        {
            var failedSmsCount = _discardSmsService.RecoveryDiscardedSmsByCampaignId(campaign.Id).Count();
            campaign.IncFailed(failedSmsCount);
            return failedSmsCount;
        }

        //metodo per gestire lo stato della campagna in base ai messaggi inviati/ falliti
 


    }
}

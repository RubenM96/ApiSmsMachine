using SmsMachine.Infrastructure.Repositories;
using SmsMachine.Interfaces;
using SmsMachine.Models;
using static SmsMachine.Models.CampaignSms;

namespace SmsMachine.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsQueueRepository _smsQueueRepository;
        private readonly ISmsService _smsService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(ICampaignRepository campaignRepository,
            ISmsQueueRepository smsQueueRepository,
            ISmsService msService, 
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsQueueRepository = smsQueueRepository;
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
            
            campaign.Status = CampaignSms.CampaignStatus.InProgress;
            _campaignRepository.UpdateCampaign(campaign);  // persisto subito i cambi

            foreach (var recipient in campaign.RecipientList.Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()))
            {
                campaign.Status = CampaignStatus.InProgress;

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

            campaign.Status = CampaignStatus.Finished;
            _campaignRepository.UpdateCampaign(campaign);
            return campaign;
        }

        // Mostra tutte le Campagne
        public IEnumerable<CampaignSms> GetAllCampaigns()
        {
            return _campaignRepository.GetAllCampaigns();
        }

        // Mostra Singola Campagna
        public CampaignSms? GetCampaignId(int id)
        {
            return _campaignRepository.GetCampaignId(id);
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

        // Elimina Campagna
        public bool DeleteCampaign(int id)
        {
            var exists = _campaignRepository.GetCampaignId(id);
            if (exists is null) return false;

            return _campaignRepository.DeleteCampaign(id);
        }


        public async Task RetryQueuedForCampaignAsync(int campaignId, TimeSpan delay)
        {
            try
            {
                do
                {
                    // 1) aspetta SENZA bloccare thread di request
                    await Task.Delay(delay);

                    // 2) carica gli elementi in coda per quella campagna
                    var queued = _smsQueueRepository.GetSmsQueueByCampaign(campaignId);

                    foreach (var smsQueue in queued)
                    {
                        try
                        {
                            _smsService.SendSms(smsQueue.Recipient.Value, smsQueue.Text, smsQueue.Multipart, smsQueue.Notify, campaignId);

                            // se l’invio è riuscito (o almeno non “queue full”), rimuovi dalla coda
                            _smsQueueRepository.Delete(smsQueue.Id);
                        }
                        catch (Exception ex)
                        {
                            //  errore: logga e lascia l’item in coda
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

    }
}

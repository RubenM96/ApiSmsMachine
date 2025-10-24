using SmsMachine.Interfaces;
using SmsMachine.Models;
using static SmsMachine.Models.CampaignSms;

namespace SmsMachine.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsService _msService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(ICampaignRepository campaignRepository, ISmsService msService, ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _msService = msService;
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
        public CampaignSms SendCampaign(int id)
        {
            CampaignSms? campaign = _campaignRepository.GetCampaignId(id);

            if(campaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            //Counter destinatari
            var recipients = campaign.RecipientList
                .Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .ToList();

            campaign.SetTotal(recipients.Count);           // imposta TotalRecipients
            campaign.Status = CampaignSms.CampaignStatus.InProgress;
            _campaignRepository.UpdateCampaign(campaign);  // persisto subito i cambi

            foreach (var recipient in campaign.RecipientList.Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()))
            {
                campaign.Status = CampaignStatus.InProgress;

                try
                {
                    _msService.SendSms(recipient, campaign.Text, false, campaign.CampaignNotify, campaign.Id);
                    _logger.LogInformation("Sent SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
            }

            //metodo per tentare l'invio dei messaggi nella coda (SmsQueue)

            campaign.Status = CampaignStatus.Finished;
            //aggiornarlo nel db
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

            existingCampaign.Title = title;
            existingCampaign.Text = text;
            existingCampaign.RecipientList = recipientList;
            existingCampaign.CampaignNotify = campaignNotify;
            existingCampaign.Description = description;
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

    }
}

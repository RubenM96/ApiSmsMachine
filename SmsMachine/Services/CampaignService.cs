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

        public CampaignSms CreateCampaign(string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            CampaignSms campaign = new CampaignSms(title, text, recipientList, campaignNotify, description);
            
            var createdCampaign = _campaignRepository.AddCampaign(campaign);
            _logger.LogInformation("Created campaign with ID {CampaignId}", createdCampaign.Id);
            return createdCampaign;
        }

        public CampaignSms SendCampaign(int id)
        {
            CampaignSms? campaign = _campaignRepository.GetCampaignId(id);

            if(campaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            foreach (var recipient in campaign.RecipientList.Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()))
            {
                campaign.Status = CampaignStatus.InProgress;

                try
                {
                    _msService.SendSms(recipient, campaign.Text, false, campaign.CampaignNotify);
                    _logger.LogInformation("Sent SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {Recipient} for Campaign ID {CampaignId}", recipient, id);
                }
            }

            campaign.Status = CampaignStatus.Finished;
            //aggiornarlo nel db
            _campaignRepository.UpdateCampaign(campaign);
            return campaign;
        }

        public IEnumerable<CampaignSms> GetAllCampaigns()
        {
            return _campaignRepository.GetAllCampaigns();
        }

        public CampaignSms? GetCampaignId(int id)
        {
            return _campaignRepository.GetCampaignId(id);
        }

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

        public bool DeleteCampaign(int id)
        {
            var exists = _campaignRepository.GetCampaignId(id);
            if (exists is null) return false;

            return _campaignRepository.DeleteCampaign(id);
        }

    }
}

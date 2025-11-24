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
        private readonly ISmsService _smsService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ISmsOutboundRepository smsOutboundRepository,
            IDiscardSmsService discardSmsService,
            ISmsService msService,           
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsOutboundRepository = smsOutboundRepository;
            _discardSmsService = discardSmsService;
            _smsService = msService;
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
                    _smsService.SendSms(smsOutbound);
                    _logger.LogInformation("Sent SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                }
            }

            return campaign;
        }

        // Modifica Campagna
        public CampaignSms UpdateCampaign(int id, CampaignForm form)
        {
            CampaignSms? existingCampaign = _campaignRepository.GetCampaignId(id);
            if (existingCampaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found for update", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            var recipients = existingCampaign.GetRecipientToList(form.RecipientList);

            _smsOutboundRepository.DeleteAllSmsByCampaignId(existingCampaign.Id);

            foreach (var recipient in recipients)
            {
                var sms = new SmsOutbound(
                    new Recipient(recipient),
                    form.Text,
                    multipart: false,
                    notify: form.CampaignNotify,
                    sentAt: DateTime.Now,
                    campaignId: existingCampaign.Id
                );

                _smsOutboundRepository.AddSms(sms);
            }

            existingCampaign.Title = form.Title;
            existingCampaign.Text = form.Text;
            existingCampaign.CampaignNotify = form.CampaignNotify;
            existingCampaign.Description = form.Description;
            existingCampaign.TotalRecipients = existingCampaign.CalculateTotalRecipients(recipients);

            var updatedCampaign = _campaignRepository.UpdateCampaign(existingCampaign);

            _logger.LogInformation("Updated campaign with ID {CampaignId}", updatedCampaign.Id);
            return updatedCampaign;
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

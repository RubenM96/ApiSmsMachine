using SmsMachine.Api.Models;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ISmsOutboundRepository smsOutboundRepository,
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _smsOutboundRepository = smsOutboundRepository;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nuova campagna SMS e i singoli messaggi per ciascun destinatario.
        /// </summary>
        /// <remarks>Se si verifica un errore durante la creazione di un singolo SMS per un destinatario, l'errore viene
        /// registrato e l'elaborazione continua per gli altri destinatari. La campagna viene creata indipendentemente
        /// dagli eventuali errori nella creazione dei singoli SMS.
        /// </remarks>
        /// <param name="campaignRequest">La richiesta contenente i dettagli della campagna, inclusi titolo, testo del messaggio, lista dei destinatari,
        /// impostazioni di notifica e descrizione. Non può essere null.</param>
        /// <returns>Un task che rappresenta l'operazione asincrona.</returns>

        public async Task CreateCampaignAsync(CreateCampaignRequest campaignRequest)
        {

            //Controlla la validità dei numeri di telefono e calcola totale dei destinatari
            RecipientList recipients = new RecipientList(campaignRequest.RecipientList);
            int totalRecipient = recipients.CalculateTotalRecipients();

            CampaignSms campaign = new CampaignSms(campaignRequest.Title, campaignRequest.Text, totalRecipient, campaignRequest.CampaignNotify, campaignRequest.Description);

            try
            {
                var createdCampaign = _campaignRepository.AddCampaign(campaign);

                //crea i singoli sms della campagna
                foreach (var recipient in recipients.Recipients)
                {
                    try
                    {
                        var sms = new SmsOutbound(new Recipient(recipient), campaign.Text, false, campaign.CampaignNotify, DateTime.Now, createdCampaign.Id);
                        _smsOutboundRepository.AddSms(sms);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create SMS for recipient {Recipient} in Campaign ID {CampaignId}", recipient, createdCampaign.Id);
                        _smsOutboundRepository.ClearErrors();
                    }
                }

                _logger.LogInformation("Created campaign with ID {CampaignId}", createdCampaign.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create campaign with title {CampaignTitle}", campaignRequest.Title);
                throw;
            }
        }

        /// <summary>
        /// Avvia il processo di invio per la campagna SMS, settando InProgress lo status della campagna e di tutti i messaggi.
        /// </summary>
        /// <remarks>Segna la campagna e i messaggi SMS in uscita associati come in corso prima dell'invio. Registra
        /// avvisi se la campagna non viene trovata e logga informazioni o errori per ciascun messaggio SMS durante
        /// l'elaborazione.</remarks>
        /// <param name="id">L'identificativo univoco della campagna da inviare.</param>
        /// <returns>Un task che rappresenta l'operazione asincrona.</returns>
        /// <exception cref="ArgumentException">Viene sollevata se non esiste alcuna campagna con l'identificativo specificato.</exception>

        public async Task SendCampaign(int id)
        {
            CampaignSms? campaign = await _campaignRepository.GetCampaignId(id);

            if (campaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            campaign.MarkInProgress();
            _campaignRepository.UpdateCampaign(campaign);

            var smsList = await _smsOutboundRepository.GetAllSmsByCampaignId(id);

            foreach (var smsOutbound in smsList)
            {
                try
                {
                    smsOutbound.MarkInProgress();
                    _smsOutboundRepository.UpdateSms(smsOutbound);
                    _logger.LogInformation("SMS to {Recipient} for Campaign ID {CampaignId} set in progress", smsOutbound.Recipient.Value, campaign.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to set SMS to {Recipient} for Campaign ID {CampaignId}", smsOutbound.Recipient.Value, campaign.Id);
                    _smsOutboundRepository.ClearErrors();
                }
            }
        }

        // Modifica Campagna
        public async Task<CampaignSms> UpdateCampaign(int id, CreateCampaignRequest form)
        {
            CampaignSms? existingCampaign = await _campaignRepository.GetCampaignId(id);
            if (existingCampaign == null)
            {
                _logger.LogWarning("Campaign with ID {CampaignId} not found for update", id);
                throw new ArgumentException($"Campaign with ID {id} not found");
            }

            var recipientList = new RecipientList(form.RecipientList);

            //cancella tutti gli sms esistenti per questa campagna
            _smsOutboundRepository.DeleteAllSmsByCampaignId(existingCampaign.Id);

            foreach (var recipient in recipientList.Recipients)
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
            existingCampaign.TotalRecipients = recipientList.CalculateTotalRecipients();

            var updatedCampaign = _campaignRepository.UpdateCampaign(existingCampaign);

            _logger.LogInformation("Updated campaign with ID {CampaignId}", updatedCampaign.Id);
            return updatedCampaign;
        }

        public async Task DeleteCampaignById(int id)
        {
            var ok = _campaignRepository.DeleteCampaign(id);
            if (ok)
            {
                _logger.LogInformation("Deleted campaign with ID {CampaignId}", id);

                //per l'integrità referenziale elimina anche tutti gli sms associati
                _smsOutboundRepository.DeleteAllSmsByCampaignId(id);
                _logger.LogInformation("Deleted all SMS for Campaign ID {CampaignId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete campaign with ID {CampaignId}", id);
            }
        }


    }
}

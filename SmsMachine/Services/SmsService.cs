using SmsMachine.Infrastructure;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly ISmsQueueService _smsQueueService;
        private readonly ILogger<SmsService> _logger;

        public SmsService(ISmsSender smsSender, ISmsOutboundRepository smsRepository, ILogger<SmsService> logger, ISmsQueueService smsQueueService)
        {
            _smsSender = smsSender;
            _smsOutboundRepository = smsRepository;
            _smsQueueService = smsQueueService;
            _logger = logger;
        }

        public AreaSxSendResult SendSms(string recipient, string text, bool multipart, bool notify, int? campaignId)
        {
            _logger.LogInformation("Preparing to send SMS to {Recipient} with text: {Text}, multipart: {Multipart}, notify: {Notify}", recipient, text, multipart, notify);

            //controlli input
            if (text.Length > 160 && !multipart)
                text = text.Substring(0, 160);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            var sms = new SmsOutbound(new Recipient(recipient), text, multipart, notify, DateTime.Now);

            if (campaignId != null)
                sms.CampaignId = campaignId;

            try
            {
                //invio sms a AreaSx
                AreaSxSendResult responeSendSms = _smsSender.SendSms(recipient, text, notify);

                if (responeSendSms.IsSuccess)
                {
                    sms.Index = responeSendSms.GetIndex();
                    _smsOutboundRepository.AddSms(sms);
                    _logger.LogInformation("SMS to {Recipient} sent and saved to database successfully", recipient);
                }
                else if (responeSendSms.Refused)
                {
                    //TODO: Gestire il messaggio rifiutato a causa della coda piena (Errore durante l'invio del messaggio:  SMS Refused by AreaSx: SMS Queue Full)                                      
                    _smsQueueService.EnqueueSms(new Recipient(recipient), text, multipart, notify, campaignId);
                    //TODO: Routing per gestire più SmsMachine

                }
                else
                {
                    throw new Exception(responeSendSms.Errno);
                }

                return responeSendSms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save SMS to database for recipient {Recipient}", recipient);
                _logger.LogError(ex, "Failed to send SMS to {Recipient}", recipient);
                throw;
            }
        }






    }
}
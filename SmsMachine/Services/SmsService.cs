using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        private readonly ISmsRepository _smsRepository;
        private readonly ILogger<SmsService> _logger;

        public SmsService(ISmsSender smsSender, ISmsRepository smsRepository, ILogger<SmsService> logger)
        {
            _smsSender = smsSender;
            _smsRepository = smsRepository;
            _logger = logger;
        }

        public void SendSms(string recipient, string text, bool multipart, bool notify)
        {
            _logger.LogInformation("Preparing to send SMS to {Recipient} with text: {Text}, multipart: {Multipart}, notify: {Notify}", recipient, text, multipart, notify);
            
            //controlli input
            if (text.Length > 160 && !multipart)
                text = text.Substring(0, 160);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            //TODO: Routing per gestire più SmsMachine

            var sms = new Sms(new Recipient(recipient), text, multipart, notify, DateTime.Now);

            try
            {
                //invio sms a AreaSx
                int index = _smsSender.SendSms(recipient, text, notify);
                sms.Index = index;

                //salvataggio su db
                _smsRepository.AddSms(sms);
                _logger.LogInformation("SMS to {Recipient} sent and saved to database successfully", recipient);

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
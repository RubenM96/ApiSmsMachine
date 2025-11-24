using SmsMachine.Infrastructure;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly ILogger<SmsService> _logger;

        public SmsService(ISmsSender smsSender, ISmsOutboundRepository smsRepository, ILogger<SmsService> logger)
        {
            _smsSender = smsSender;
            _smsOutboundRepository = smsRepository;
            _logger = logger;
        }

        public AreaSxSendResult SendSms(SmsOutbound smsOutbound)
        {
            if (smsOutbound == null)
                throw new ArgumentNullException(nameof(smsOutbound));
      
            _logger.LogInformation(
                "Preparing to send SMS to {Recipient}. Text: {Text}, multipart: {Multipart}, notify: {Notify}",
                smsOutbound.Recipient.Value,
                smsOutbound.Text,
                smsOutbound.Multipart,
                smsOutbound.Notify
            );
    
            var text = smsOutbound.Text;

            if (text.Length > 160 && !smsOutbound.Multipart)
                text = text.Substring(0, 160);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            // aggiorno l'entità con il testo normalizzato 
            smsOutbound.Text = text;

            //chiamata macchina 
            var responseSendSms = _smsSender.SendSms(
                smsOutbound.Recipient.Value,
                smsOutbound.Text,
                smsOutbound.Notify
            );

            
            if (responseSendSms.IsSuccess)
            {
                smsOutbound.SentAt = DateTime.Now;
                smsOutbound.Status = SmsStatus.Sent;
                smsOutbound.Index = responseSendSms.GetIndex();

                if (smsOutbound.Id == 0 || smsOutbound.Id == null)
                    _smsOutboundRepository.AddSms(smsOutbound);
                else
                    _smsOutboundRepository.UpdateSms(smsOutbound);

                return responseSendSms;
            }
            else if (responseSendSms.Refused)
            {
                smsOutbound.SentAt = DateTime.Now;
                smsOutbound.Status = SmsStatus.InProgress;

                if (smsOutbound.Id == 0 || smsOutbound.Id == null)
                    _smsOutboundRepository.AddSms(smsOutbound);
                else
                    _smsOutboundRepository.UpdateSms(smsOutbound);

                return responseSendSms;
            }
            else
            {
                throw new Exception(responseSendSms.Errno);
            }
        }


    }
}
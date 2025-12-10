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

        /// <summary>
        /// Invia un messaggio SMS utilizzando i dettagli dell'SMS in uscita specificati e restituisce il risultato dell'operazione di invio.
        /// </summary>
        /// <remarks>
        /// Il metodo aggiorna inoltre il repository degli SMS per riflettere lo stato del messaggio.
        /// </remarks>
        /// <param name="smsOutbound">I dettagli dell'SMS da inviare. Non può essere null. Il testo del messaggio può essere troncato in base alla lunghezza e alle impostazioni di multipart.</param>
        /// <returns>Un oggetto AreaSxSendResult contenente il risultato dell'operazione di invio SMS. Il risultato indica se il messaggio è stato inviato con successo, rifiutato o non riuscito.</returns>
        /// <exception cref="ArgumentNullException">Viene sollevata se smsOutbound è null.</exception>
        /// <exception cref="Exception">Viene sollevata se l'operazione di invio SMS fallisce e la risposta non indica successo o rifiuto. Il messaggio dell'eccezione contiene il codice di errore restituito dal mittente SMS.</exception>

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

            //chiamata macchina AreaSx SmsMachine
            var responseSendSms = _smsSender.SendSms(
                smsOutbound.Recipient.Value,
                smsOutbound.Text,
                smsOutbound.Notify
            );
          
            if (responseSendSms.IsSuccess)
            {
                smsOutbound.MarkSent(responseSendSms.GetIndex());

                if (smsOutbound.Id == 0 || smsOutbound.Id == null)
                    _smsOutboundRepository.AddSms(smsOutbound);
                else
                    _smsOutboundRepository.UpdateSms(smsOutbound);

                return responseSendSms;
            }
            else if (responseSendSms.Refused)
            {
                smsOutbound.MarkInProgress();

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
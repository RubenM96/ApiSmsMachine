namespace SmsMachine.Services
{
    public class AreaSxSmsReceiver : ISmsReceiver
    {
        private readonly ILogger<AreaSxSmsReceiver> _logger;
        private readonly ISmsInbound _smsInboundService;
        private readonly INotifyService _notifyService;

        public AreaSxSmsReceiver(ILogger<AreaSxSmsReceiver> logger, INotifyService notifyService, ISmsInbound smsInboundService) 
        { 
            _logger = logger;
            _notifyService = notifyService;
            _smsInboundService = smsInboundService;
        }

        public void Receive(string index, string recipient, string text, string date)
        {
            //controllare se notifica o sms
            if(IsNotifica(text))
            {
                //notifica di ricezione
                _logger.LogInformation("Notifica sms ricevuta: {Index}, recipient {Recipient}, text {Text}, date {Date}", index, recipient, text, date);                              
                _notifyService.Notify(index, recipient, text, date);
            }
            else
            {
                //Registra sms
                _logger.LogInformation("Receveing SMS from {Recipient} with text: {Text}", recipient, text);

                if (string.IsNullOrEmpty(recipient))
                    throw new ArgumentException("Numero mancante");
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("Testo del messaggio mancante");

                _smsInboundService.SmsInbound(recipient, text, date);

            }
        }

        private bool IsNotifica(string text)
        {
            return text == "STATUS REPORT";
        }

    }
}
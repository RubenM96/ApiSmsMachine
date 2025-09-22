
namespace SmsMachine.Services
{
    public class AreaSxSmsReceiver : ISmsReceiver
    {
        private readonly ILogger<AreaSxSmsReceiver> _logger;
        private readonly ISmsInbound _smsInboundService;

        public AreaSxSmsReceiver(ILogger<AreaSxSmsReceiver> logger, ISmsInbound smsInboundService)
        {
            _logger = logger;
            _smsInboundService = smsInboundService;
        }

        public void Receive(string index, string recipient, string text, string date)
        {
            //controllare se notifica o sms
            if(IsNotifica(text))
            {
                //Registra Notifica
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
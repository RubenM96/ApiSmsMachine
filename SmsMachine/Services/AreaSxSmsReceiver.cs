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

        public void Receive(
            string code, string recipient, string text, string date,
            string? sms_id, string? sms_totparts, string? sms_thispart,
            string? sms_status)
        {
            //controlla se è una notifica o un sms
            if (IsNotifica(text))
            {

                if (string.IsNullOrWhiteSpace(sms_id))
                    throw new ArgumentException("Index empty");

                if (string.IsNullOrWhiteSpace(sms_status))
                    throw new ArgumentException("Status empty");

                _notifyService.Notify(code, recipient, text, date, int.Parse(sms_id), sms_status);

            }
            else
            {
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
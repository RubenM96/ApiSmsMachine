namespace SmsMachine.Services
{
    public class AreaSxSmsReceiver : ISmsReceiver
    {
        private readonly ILogger<AreaSxSmsReceiver> _logger;
        private readonly INotifyService _notifyService;

        public AreaSxSmsReceiver(ILogger<AreaSxSmsReceiver> logger, INotifyService notifyService) 
        { 
            _logger = logger;
            _notifyService = notifyService;
        }

        public void Receive(string index, string recipient, string text, string date)
        {
            //controllare se notifica o sms
            if(IsNotifica(text))
            {
                //notifica di ricezione sms
                _logger.LogInformation("Notifica sms ricevuta: {Index}, recipient {Recipient}, text {Text}, date {Date}", index, recipient, text, date);                              
                _notifyService.Notify(index, recipient, text, date);
            }
            else
            {
                _logger.LogInformation("Sms ricevuto con index {Index}, recipient {Recipient}, text {Text}, date {Date}", index, recipient, text, date);
                //Registra sms
                //TODO: creare service per smsReceiver

            }
        }

        private bool IsNotifica(string text)
        {
            return text == "STATUS REPORT";
        }

    }
}
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

        /// <summary>
        /// Elabora un SMS o un messaggio di notifica in ingresso e lo instrada
        /// al servizio appropriato in base al suo contenuto.
        /// </summary>
        /// <remarks>
        /// Il metodo IsNotifica distingue tra notifiche e SMS basandosi sul contenuto
        /// del parametro 'text'. Le notifiche vengono inoltrate al servizio di
        /// gestione notifiche, mentre gli SMS vengono inviati al servizio di
        /// gestione degli SMS in ingresso.
        /// </remarks>
        /// <param name="code">
        /// Password di accesso allo script Web (se è richiesta)
        /// </param>
        /// <param name="recipient">
        /// Il numero di telefono o l’identificatore del destinatario.
        /// </param>
        /// <param name="text">
        /// Il contenuto testuale del messaggio.
        /// </param>
        /// <param name="date">
        /// La data e l’ora in cui il messaggio è stato ricevuto, rappresentata come stringa.
        /// </param>
        /// <param name="sms_id">
        /// L’identificatore dell’SMS (index). Non può essere nullo o vuoto
        /// quando si elabora una notifica.
        /// </param>
        /// <param name="sms_totparts">
        /// Il numero totale di parti se l’SMS è multipart. Opzionale.
        /// </param>
        /// <param name="sms_thispart">
        /// L’indice di questa parte in un SMS multipart. Opzionale.
        /// </param>
        /// <param name="sms_status">
        /// Lo stato dell’SMS. Necessario quando si elabora una notifica.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Generata quando i parametri richiesti sono mancanti o vuoti, ad esempio
        /// quando 'recipient' o 'text' sono vuoti per gli SMS, oppure 'sms_id'
        /// o 'sms_status' sono vuoti per le notifiche.
        /// </exception>

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

                _notifyService.Notify(recipient, text, date, int.Parse(sms_id), sms_status);

            }
            else
            {
                _logger.LogInformation("Receveing SMS from {Recipient} with text: {Text}", recipient, text);

                if (string.IsNullOrEmpty(recipient))
                    throw new ArgumentException("Recipient Empty");
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("Sms Text Empty");

                _smsInboundService.SmsInbound(recipient, text, date);

            }
        }

        private bool IsNotifica(string text)
        {
            return text == "STATUS REPORT";
        }

    }
}
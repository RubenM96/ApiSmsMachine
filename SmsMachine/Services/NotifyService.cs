using Serilog;
using SmsMachine.Infrastructure.Utils;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services;

public class NotifyService : INotifyService
{

    private readonly ILogger<NotifyService> _logger;
    private readonly INotifyRepository _notifyRepository;
    private readonly ISmsOutboundRepository _smsOutboundRepository;

    public NotifyService(ILogger<NotifyService> logger, INotifyRepository notifyRepository, ISmsOutboundRepository smsOutboundRepository)
    {
        _logger = logger;
        _notifyRepository = notifyRepository;
        _smsOutboundRepository = smsOutboundRepository;
    }


    /// <summary>
    /// Crea e memorizza una notifica per un SMS associato al destinatario e all'indice specificati.
    /// </summary>
    /// <remarks>Il metodo registra informazioni sulla notifica e la memorizza nel repository delle notifiche. 
    /// </remarks>
    /// <param name="recipient">Il numero di telefono del destinatario per il quale viene creata la notifica.</param>
    /// <param name="text">Il testo della notifica "STATUS REPORT".</param>
    /// <param name="date">La data e l'orario della notifica, rappresentati come stringa. Il formato deve essere compatibile con il parser delle date previsto.</param>
    /// <param name="indexSms">L'id del SMS per il quale viene creata la notifica.</param>
    /// <param name="status">Il valore di stato da assegnare alla notifica, indicando il suo stato corrente.</param>
    /// <exception cref="ArgumentException">Viene sollevata se non viene trovato alcun messaggio SMS in uscita per il destinatario e l'indexSms specificati.</exception>

    public void Notify( string recipient, string text, string date, int indexSms, string status)
    {

        SmsOutbound? smsOutbound = _smsOutboundRepository.GetSmsOutboundByRecipientAndIndex(recipient, indexSms);

        if (smsOutbound == null)
        {
            _logger.LogWarning("Not found SmsOutbound for recipient {Recipient} and index {Index}", recipient, indexSms.ToString());
            Log.ForContext("Notify", "NotifyNotSent").Error("Not found SmsOutbound for recipient {Recipient} and index {Index}", recipient, indexSms.ToString());

            throw new ArgumentException("Sms not found");
        }

        DateTime dateTime = SmsDateParser.ParseDateNotify(date);

        var notify = new Notify(new Recipient(recipient), text, dateTime, indexSms, status, smsOutbound.Id);

        _logger.LogInformation("Info Notify: num {Recipient} text: {Text}, date {Date}, index {Index}, status {Status}, smsId {smsOutbound.Id} ", recipient, text, dateTime, indexSms.ToString(), status, smsOutbound.Id);

        _notifyRepository.AddNotify(notify);

    }

}

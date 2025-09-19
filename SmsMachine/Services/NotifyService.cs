using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services;

public class NotifyService : INotifyService
{

    private readonly ILogger<NotifyService> _logger;
    private readonly INotifyRepository _notifyRepository;

    public NotifyService(ILogger<NotifyService> logger, INotifyRepository notifyRepository)
    {
        _logger = logger;
        _notifyRepository = notifyRepository;
    }

    public void Notify(string index, string recipient, string text, string date)
    {
        //aggiungere i valori in un oggetto Notify per passarlo al repository
        //System.FormatException: String = '2025-09-19 13:16:58 GMT +02' was not recognized as a valid DateTime.
        date = date.Replace("-", "/").Trim();
        date = date.Replace(" +", "").Trim(); //per gestire il caso +2
        date = date.Replace("GMT", "").Trim();
        DateTime dateTime = DateTime.ParseExact(date, "yyyy/MM/dd HH:mm:ss fff", System.Globalization.CultureInfo.InvariantCulture);


        var notify = new Notify(new Recipient(recipient), text, dateTime);     

        //chiama il repository per salvare la notifica sul db
        _notifyRepository.AddNotify(notify);

    }

}

using SmsMachine.Infrastructure.Utils;
using SmsMachine.Interfaces;
using SmsMachine.Models;
using System.Globalization;

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

    public void Notify(string index, string recipient, string text, string date, int indexSms, string status)
    {
        DateTime dateTime = SmsDateParser.ParseDateNotify(date);
        var notify = new Notify(new Recipient(recipient), text, dateTime, indexSms, status);     
        
        _logger.LogInformation("Info Notify: num {Recipient} text: {Text}, date {Date}, index {Index}, status {Status}", recipient, text, dateTime, indexSms.ToString() , status);        
        _notifyRepository.AddNotify(notify);

    }

}

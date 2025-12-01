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

    public void Notify(string index, string recipient, string text, string date, int indexSms, string status)
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

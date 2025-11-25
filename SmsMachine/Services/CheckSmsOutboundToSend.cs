using SmsMachine.Interfaces;
using SmsMachine.Services;

namespace SmsMachine.Api.Services;

public class CheckSmsOutboundToSend : ICheckSmsOutboundToSend
{
    private readonly ISmsOutboundRepository _smsOutboundRepository;
    private readonly ISmsService _smsService;
    private readonly ILogger<CheckSmsOutboundToSend> _logger;

    public CheckSmsOutboundToSend(
        ISmsOutboundRepository smsOutboundRepository,
        ISmsService smsService,
        ILogger<CheckSmsOutboundToSend> logger)
    {
        _smsOutboundRepository = smsOutboundRepository;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task CheckSmsOutboundInProgress()
    {
        _logger.LogInformation("Controllo SMS in stato InProgress da inviare...");
        var smsOutboundInProgress = _smsOutboundRepository.GetSmsOutboundInProgress();

        if (smsOutboundInProgress != null) 
        { 
            _logger.LogInformation("Invio SMS in stato InProgress con Id: {SmsId}", smsOutboundInProgress.Id);
            _smsService.SendSms(smsOutboundInProgress);
        }

    }



}

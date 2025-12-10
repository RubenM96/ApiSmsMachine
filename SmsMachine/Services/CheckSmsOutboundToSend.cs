using Serilog;
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

    /// <summary>
    /// Verifica la presenza di messaggi SMS con lo stato InProgress da inviare.
    /// </summary>
    /// <remarks>Se esiste un SMScon stato 'InProgress', verrà inviato utilizzando il servizio SMS configurato. 
    /// Non viene intrapresa alcuna azione se non viene trovato alcun messaggio di questo tipo.
    /// </remarks>

    public async Task CheckSmsOutboundInProgress()
    {
        _logger.LogInformation("Controllo SMS in stato InProgress da inviare...");
        var smsOutboundInProgress = _smsOutboundRepository.GetSmsOutboundInProgress();

        if (smsOutboundInProgress != null) 
        { 
            _logger.LogInformation("Invio SMS in stato InProgress con Id: {SmsId}", smsOutboundInProgress.Id);
            Log.ForContext("SmsType", "Sent").Information("Send SMS whit status InProgress for Id: {SmsId}", smsOutboundInProgress.Id);
            _smsService.SendSms(smsOutboundInProgress);
        }

    }


}

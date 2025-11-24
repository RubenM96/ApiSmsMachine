using SmsMachine.Interfaces;
using SmsMachine.Services;

namespace SmsMachine.Api.Services;

public class CheckSmsOutboundToSend
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


    }



}

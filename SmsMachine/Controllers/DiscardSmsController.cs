using Microsoft.AspNetCore.Mvc;
using SmsMachine.Api.Services;
using SmsMachine.Interfaces;
using SmsMachine.Services;


namespace SmsMachine.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DiscardSmsController : ControllerBase
    {
        private readonly ISmsDiscard _smsDiscard;
        private readonly IDiscardSmsService _discardSmsService;
        private readonly ILogger<DiscardSmsController> _logger;

        public DiscardSmsController(ISmsDiscard smsDiscard, 
            IDiscardSmsService discardSmsService,
            ILogger<DiscardSmsController> logger)
        {
            _smsDiscard = smsDiscard;
            _discardSmsService = discardSmsService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public IActionResult CheckDiscardSms()
        {
            try
            {
                var result = _smsDiscard.SmsNotSend();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discard SMS Request");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpGet("[action]")]
        public IActionResult RecoveryDiscardedSmsFromCampaign(int campaignId)
        {
            try
            {
                var result = _discardSmsService.RecoveryDiscardedSmsByCampaignId(campaignId);

                if (!result.Any())
                {
                    _logger.LogInformation("No discarded SMS to recover.");
                    return Ok("No discarded SMS to recover.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while recovering discarded SMS.");
                return StatusCode(500);
            }
        }
    }
}

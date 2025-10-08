using Microsoft.AspNetCore.Mvc;
using SmsMachine.Services;

namespace SmsMachine.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DiscardSmsController : ControllerBase
    {
        private readonly ISmsDiscard _smsDiscard;
        private readonly ILogger<DiscardSmsController> _logger;

        public DiscardSmsController(ISmsDiscard smsDiscard, ILogger<DiscardSmsController> logger)
        {
            _smsDiscard = smsDiscard;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CheckDiscardSms()
        {
            try
            {
                var result = _smsDiscard.CheckDiscardSms();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discard SMS Request");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}

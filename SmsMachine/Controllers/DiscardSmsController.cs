using Microsoft.AspNetCore.Mvc;
using SmsMachine.Api.Services;
using SmsMachine.Services;


namespace SmsMachine.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DiscardSmsController : ControllerBase
    {
        private readonly IDiscardSmsService _discardSmsService;


        public DiscardSmsController(
            IDiscardSmsService discardSmsService)
        {

            _discardSmsService = discardSmsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> RecoveryDiscardedSmsFromCampaign(int campaignId)
        {
            var result = await _discardSmsService.RecoveryDiscardedSmsByCampaignId(campaignId);
            return Ok(result);       
        }

    }
}

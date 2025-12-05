using Microsoft.AspNetCore.Mvc;
using SmsMachine.Interfaces;

namespace SmsMachine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SmsOutboundController : ControllerBase
    {
        private readonly ISmsOutboundRepository _smsOutboundRepository;
        private readonly ILogger<SmsOutboundController> _logger;

        public SmsOutboundController(
            ISmsOutboundRepository smsOutboundRepository,
            ILogger<SmsOutboundController> logger)
        {
            _smsOutboundRepository = smsOutboundRepository;
            _logger = logger;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [HttpGet("{campaignId}")]
        public async Task<IActionResult> GetAllSmsOutboundByCampaignId(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                    return BadRequest("CampaignId non valido.");

                var smsList = await _smsOutboundRepository.GetAllSmsByCampaignId(campaignId);

                if (smsList == null || smsList.Count == 0)
                    return NotFound("Nessun SMS trovato per questa campagna.");

                return Ok(smsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli SMS per la campagna {CampaignId}", campaignId);
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

    }
}

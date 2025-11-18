using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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


        [HttpPost("[action]")]
        public IActionResult GetAllSmsOutboundByCampaignId(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                    return BadRequest("CampaignId non valido.");

                var smsList = _smsOutboundRepository.GetAllSmsByCampaignId(campaignId);

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

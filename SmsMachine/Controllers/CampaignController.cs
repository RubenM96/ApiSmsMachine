using Microsoft.AspNetCore.Mvc;
using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Interfaces;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(ICampaignService campaignService,
            ICampaignRepository campaignRepository,
            ILogger<CampaignController> logger)
        {
            _campaignService = campaignService;
            _campaignRepository = campaignRepository;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public IActionResult CreateCampaign([FromForm] CampaignForm campaignReceiver)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdCampaign = _campaignService.CreateCampaign(campaignReceiver.Title, campaignReceiver.Text, campaignReceiver.RecipientList, campaignReceiver.CampaignNotify, campaignReceiver.Description);
                return Ok(createdCampaign);
            }
            catch (ArgumentException ex) when (ex.ParamName is "recipient" or "recipientList")
            {
                ModelState.AddModelError("RecipientList", ex.Message);
                _logger.LogWarning(ex, "Error validation recipientList");
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating campaign: {ex.Message}");
                return BadRequest($"Errore nella creazione della campagna: {ex.Message}");
            }
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> SendCampaign(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var campaignSend = await _campaignService.SendCampaign(id);
                return Ok(campaignSend);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending campaign: {ex.Message}");
                return BadRequest($"Error sending campaign: {ex.Message}");
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetAllCampaigns()
        {
            try
            {
                var getAllCampaigns = _campaignRepository.GetAllCampaigns();
                return Ok(getAllCampaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving campaigns: {ex.Message}");
                return BadRequest($"Error retrieving campaigns: {ex.Message}");
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PagedResult<CampaignListDTO>>> Search([FromQuery] CampaignFilter filter)
        {
            var result = await _campaignService.SearchAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCampaign(int id)
        {
            var getOneCampaign = _campaignRepository.GetCampaignId(id);
            return Ok(getOneCampaign);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCampaign(int id, [FromForm] CampaignForm campaignReceiver)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCampaign = _campaignService.UpdateCampaign(id, campaignReceiver.Title, campaignReceiver.Text, campaignReceiver.RecipientList, campaignReceiver.CampaignNotify, campaignReceiver.Description);
                return Ok(updatedCampaign);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating campaign: {ex.Message}");
                return BadRequest($"Error updating campaign: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCampaign(int id)
        {
            try
            {
                var ok = _campaignRepository.DeleteCampaign(id);
                if (!ok) return NotFound($"Campaign {id} not found");
                return Ok("Campaign delete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting campaign {Id}", id);
                return BadRequest($"Error deleting campaign: {ex.Message}");
            }
        }


    }
}

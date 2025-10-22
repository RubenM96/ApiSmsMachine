using Microsoft.AspNetCore.Mvc;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger)
        {
            _campaignService = campaignService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateCampaign([FromForm] CampaignForm campaignReceiver)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdCampaign = _campaignService.CreateCampaign(campaignReceiver.Title, campaignReceiver.Text, campaignReceiver.RecipientList, campaignReceiver.CampaignNotify, campaignReceiver.Description);
                return Ok(createdCampaign);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating campaign: {ex.Message}");
                return BadRequest($"Error creating campaign: {ex.Message}");
            }
        }

        [HttpPost("{id}/send")]
        public IActionResult SendCampaign(int id)
        {         
            var campaignSend = _campaignService.SendCampaign(id);
            return Ok(campaignSend);
        }

        //richiesta get per visualizzare tutte le campagne GettAll
        [HttpGet]
        public IActionResult GetAllCampaigns()
        {
            var getAllCampaigns = _campaignService.GetAllCampaigns();
            return Ok(getAllCampaigns);
        }

        //richiesta get per visualizzare la singola campagna {id}
        [HttpGet("{id}/view")]
        public IActionResult GetCampaign(int id)
        {
            var getOneCampaign = _campaignService.GetCampaignId(id);
            return Ok(getOneCampaign);
        }

        //richiesta put per modificare una campagna {id}
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

        //richiesta delete per eliminare una campagna {id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCampaign(int id) 
        {
            try
            {
                var ok = _campaignService.DeleteCampaign(id);
                if (!ok) return NotFound($"Campagna {id} non trovata");
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting campaign {Id}", id);
                return BadRequest($"Error deleting campaign: {ex.Message}");
            }
        }
    }
}

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
                _logger.LogInformation("Creating new campaign");
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

        //richiesta get per visualizzare le campagne 

        //richiesta get per visualizzare i dettagli di una singola campagna {id}
        [HttpGet("{id}/view")]
        public IActionResult GetCampaign(int id)
        {
            var getOneCampaign = _campaignService.GetCampaign(id);
            return Ok(getOneCampaign);
        }
        //richiesta put per modificare una campagna {id}

        //richiesta delete per eliminare una campagna {id}

    }
}

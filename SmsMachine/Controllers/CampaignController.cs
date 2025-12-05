using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmsMachine.Api.Models;
using SmsMachine.Api.Models.DTO;
using SmsMachine.Api.Services.Queries;
using SmsMachine.Interfaces;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILogger<CampaignController> _logger;
        private readonly ICampaignQueryService _campaignQueryService;

        public CampaignController(
            ICampaignService campaignService,
            ICampaignQueryService campaignQueryService,
            ICampaignRepository campaignRepository,
            ILogger<CampaignController> logger)
        {
            _campaignService = campaignService;
            _campaignQueryService = campaignQueryService;
            _campaignRepository = campaignRepository;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
               await _campaignService.CreateCampaignAsync(request);
               return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating campaign: {ex.Message}");
                return BadRequest($"Errore nella creazione della campagna: {ex.Message}");
            }
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("{id}")]
        public async Task<IActionResult> SendCampaign(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _campaignService.SendCampaign(id);
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending campaign: {ex.Message}");
                return BadRequest($"Error sending campaign: {ex.Message}");
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CampaignListDTO>>> Search([FromBody] CampaignFilter filter)
        {
            var result = await _campaignQueryService.SearchAsync(filter);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaign(int id)
        {
            var getOneCampaign = await _campaignRepository.GetCampaignId(id);
            return Ok(getOneCampaign);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignProgressDTO>> GetProgress(int id)
        {
            var dto = _campaignQueryService.GetCampaignProgress(id);
            return Ok(dto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] CreateCampaignRequest campaignReceiver)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCampaign = await _campaignService.UpdateCampaign(id, campaignReceiver);
                return Ok(updatedCampaign);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating campaign: {ex.Message}");
                return BadRequest($"Error updating campaign: {ex.Message}");
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {        
             await _campaignService.DeleteCampaignById(id);
             return Ok();           
        }

    }
}

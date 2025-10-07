using Microsoft.AspNetCore.Mvc;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReceiverController : ControllerBase
    {
        private readonly ISmsReceiver _smsReceiver;
        private readonly ILogger<ReceiverController> _logger;

        public ReceiverController(ISmsReceiver smsReceiver, ILogger<ReceiverController> logger)
        {
            _smsReceiver = smsReceiver;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult ReceiveSms([FromForm] SmsReceived sms)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Sms o notifica arrivata!");

                _logger.LogInformation(
                    $"Informazioni complete: {sms.sms_code}, {sms.sms_num}, {sms.sms_text}, {sms.sms_date}\n" +
                    $"Attributi Sms esteso: {sms.sms_id}, {sms.sms_totparts}, {sms.sms_thispart},\n" +
                    $"Status: {sms.sms_status}"
                    );

                _smsReceiver.Receive(sms.sms_code, sms.sms_num, sms.sms_text, sms.sms_date, sms.sms_id, sms.sms_totparts, sms.sms_thispart, sms.sms_status);

            }catch (Exception ex)
            {
                return BadRequest($"Errore durante la ricezzione del messaggio: {ex.Message}");
            }

            return Ok();
        }

    }
}

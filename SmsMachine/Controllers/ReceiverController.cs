using Microsoft.AspNetCore.Http;
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
            _logger.LogInformation("Sms o notifica arrivata!");
            _smsReceiver.Receive(sms.sms_code, sms.sms_num, sms.sms_text, sms.sms_date, sms.sms_id, sms.sms_totparts, sms.sms_thispart);
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers;

[ApiController]
[Route("api/sendmessage")]
public class SendSmsController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SendSmsController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost]
    public IActionResult Send([FromForm] SmsSend sms)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // costruisco l’oggetto e lo passo al service
            var smsOutbound = new SmsOutbound(
                 new Recipient(sms.Recipient),
                 sms.Text,
                 multipart: false,
                 notify: sms.Notify.Value,
                 sentAt: DateTime.Now,
                 campaignId: null
             );

            var responeSendSms = _smsService.SendSms(smsOutbound);
            return Ok(responeSendSms);
        }
        catch (Exception ex)
        {
            return BadRequest($"Errore durante l'invio del messaggio: {ex.Message}");
        }
    }
}

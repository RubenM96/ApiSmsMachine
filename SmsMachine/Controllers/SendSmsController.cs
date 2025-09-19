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
            _smsService.SendSms(sms.Recipient, sms.Text, false, sms.Notify.Value);
        }
        catch (Exception ex)
        {
            return BadRequest($"Errore durante l'invio del messaggio: {ex.Message}");
        }

        return Ok("Messaggio inviato con successo!");

    }
}

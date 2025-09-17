using Microsoft.AspNetCore.Mvc;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers;

[ApiController]
[Route("api/sendmessage")]
public class SendSmsController: ControllerBase
{
    private readonly ISmsService _smsService;

    public SendSmsController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost]
    public IActionResult Send([FromForm] SmsSend sms)
    {
        if (ModelState.IsValid)
        {
            _smsService.SendSms(sms.Recipient, sms.Text, false, sms.Notify.Value);
        }

        return BadRequest(ModelState);
    }
}

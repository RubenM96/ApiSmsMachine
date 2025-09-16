using Microsoft.AspNetCore.Mvc;
using SmsMachine.Models;
using SmsMachine.Services;

namespace SmsMachine.Controllers;

[ApiController]
[Route("api/sendmessage")]
public class SendSmsController: ControllerBase
{
    private readonly ISmsSender _smsSender;

    public SendSmsController(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }

    [HttpPost]
    public IActionResult Send([FromForm] SmsSend sms)
    {
        if(ModelState.IsValid)
        {
            var index = _smsSender.SendSms(sms.Recipient, sms.Text, sms.Notify.Value);
        }

        return BadRequest(ModelState);
    }

}

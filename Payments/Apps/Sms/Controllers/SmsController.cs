using Microsoft.AspNetCore.Mvc;
using Payments.Apps.AppSystem.Controllers;
using Payments.Apps.Sms.Helpers;

namespace Payments.Apps.Sms.Controllers
{
    [ApiController]
    [Route("api/sms")]
    public class SmsController : ControllerBase
    {

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSMS([FromBody] DTOs.SmsDto smsDto)
        {
            if (smsDto == null || string.IsNullOrEmpty(smsDto.Phone) || string.IsNullOrEmpty(smsDto.Message))
            {
                return BadRequest("Phone and message are required.");
            }

            string sendSmsResponse = await SmsHelper.SendSMS(smsDto.Phone, smsDto.Message);

            if (sendSmsResponse == "Error: SMS not sent.")
            {
                return BadRequest("Error: SMS not sent.");
            }

            return Ok(sendSmsResponse);
        }

        [HttpGet("all-sms")]
        public IActionResult GetSmsList()
        {
            var smsList = SmsHelper.GetSmsList();
            return Ok(smsList);
        }

    }

}

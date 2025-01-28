using Microsoft.AspNetCore.Mvc;
using Payments.Apps.kyc.Interfaces;
using Payments.DTOs;
using static Payments.DTOs.OrgDto;

namespace Payments.Apps.kyc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KycController : ControllerBase
    {
        private readonly IKycService _kycService;

        public KycController(IKycService kycService)
        {
            _kycService = kycService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateKyc([FromBody] CreateKycDto createKycDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid request data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _kycService.CreateKycAsync(createKycDto);

            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(detail: errors.ToString(), statusCode: 400)
            );
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateKycStatus([FromBody] UpdateKycStatusDto updateKycStatusDto)
        {
            if (string.IsNullOrEmpty(updateKycStatusDto.KycId) || string.IsNullOrEmpty(updateKycStatusDto.NewStatus))
            {
                return BadRequest(new
                {
                    Message = "KycId and NewStatus cannot be null or empty."
                });
            }

            var result = await _kycService.UpdateKycStatusAsync(updateKycStatusDto.KycId, updateKycStatusDto.NewStatus);
            return result.Match<IActionResult>(
                kyc => Ok(kyc),
                errors => Problem(errors.ToString())
            );
        }

    }

}
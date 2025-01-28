

using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Payments.Apps.Org.Interfaces;
using Payments.Apps.Org.Models;
using static Payments.DTOs.OrgDto;

namespace Payments.Apps.Org.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrgController : ControllerBase
    {
        private readonly IOrgService _orgService;

        public OrgController(IOrgService orgService)
        {
            _orgService = orgService;
        }

        [HttpPost("create-org")]
        public async Task<IActionResult> CreateOrg([FromBody] CreateOrgDto createOrgDto)
        {
            if (createOrgDto == null)
            {
                return BadRequest("Organization data cannot be null.");
            }

            ErrorOr<OrgModel> result = await _orgService.CreateOrg(createOrgDto);
            return result.Match<IActionResult>(
                success => CreatedAtAction(nameof(GetOrgById), new { id = success.Id }, success),
                errors => Problem(errors.ToString())
            );
        }

        [Authorize("Admin")]
        [HttpGet("get-org/{id}")]
        public async Task<IActionResult> GetOrgById(string id)
        {
            ErrorOr<OrgModel> result = await _orgService.GetOrgById(id);
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [HttpGet("all-orgs")]
        public async Task<IActionResult> GetAllOrgs()
        {
            ErrorOr<List<OrgModel>> result = await _orgService.GetAllOrgs();
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [HttpPut("update-org/{id}")]
        public async Task<IActionResult> UpdateOrg(string id, [FromBody] UpdateOrgDto updateOrgDto)
        {
            if (updateOrgDto == null)
            {
                return BadRequest("Updated organization data cannot be null.");
            }

            ErrorOr<OrgModel> result = await _orgService.UpdateOrg(id, updateOrgDto);
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [HttpDelete("delete-org/{id}")]
        public async Task<IActionResult> DeleteOrg(string id)
        {
            ErrorOr<bool> result = await _orgService.DeleteOrg(id);
            return result.Match<IActionResult>(
                success => Ok(),
                errors => Problem(errors.ToString())
            );
        }

    }

}
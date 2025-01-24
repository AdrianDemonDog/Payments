using ErrorOr;
using Payments.Apps.Org.Models;
using static Payments.DTOs.OrgDto;

namespace Payments.Apps.Org.Interfaces
{
    public interface IOrgService
    {
        Task<ErrorOr<OrgModel>> CreateOrg(CreateOrgDto createOrgDto);
        Task<ErrorOr<OrgModel>> GetOrgById(string id);
        Task<ErrorOr<List<OrgModel>>> GetAllOrgs();
        Task<ErrorOr<OrgModel>> UpdateOrg(string id, UpdateOrgDto updateOrgDto);
        Task<ErrorOr<bool>> DeleteOrg(string id);
    }
}

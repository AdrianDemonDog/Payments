using ErrorOr;
using Payments.Apps.Org.Models;
using Payments.DTOs;
using static Payments.DTOs.OrgDto;

namespace Payments.Apps.kyc.Interfaces
{
    public interface IKycService
    {
        Task<ErrorOr<KycModel>> CreateKycAsync(OrgDto.CreateKycDto kycModel);
        Task<ErrorOr<KycModel>> UpdateKycStatusAsync(string kycId, string newStatus);
    }
}

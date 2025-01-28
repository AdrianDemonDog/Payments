namespace Payments.DTOs
{
    public class OrgDto
    {
        public class CreateOrgDto
        {
            public string? Name { get; set; }
            public string? TaxNumber { get; set; }
            public string? CreatedBy { get; set; }
            public string? Instance { get; set; }
            public string? KYC { get; set; }
            public string? LegalName { get; set; }
        }

        public class UpdateOrgDto
        {
            public string? Name { get; set; }
            public string? TaxNumber { get; set; }
            public string? Instance { get; set; }
            public string? KYC { get; set; }
            public string? LegalName { get; set; }
            public DateTime? KYCStatusChanged { get; set; }
            public string? UpdatedBy { get; set; }
        }

        public class CreateKycDto
        {
            public int CompanyId { get; set; }
            public string? Status { get; set; }
            public CompanyDataDto? CompanyData { get; set; }
            public GroupDataDto? GroupData { get; set; }
        }

        public class CompanyDataDto
        {
            public string? LegalName { get; set; }
            public string? OrgaNumber { get; set; }
            public string? Country { get; set; }
            public string? Address { get; set; }
            public string? Representative { get; set; }
            public string? PNRepresentative { get; set; }
            public string? RepresentativeFiles { get; set; }
            public string? TravelTourismFiles { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Industry { get; set; }
            public List<string>? Reason { get; set; }
            public List<string>? Nature { get; set; }
        }

        public class GroupDataDto
        {
            public int CompanyGroup { get; set; }
        }

        public class UpdateKycStatusDto
        {
            public string? KycId { get; set; }
            public string? NewStatus { get; set; }
        }

    }

}
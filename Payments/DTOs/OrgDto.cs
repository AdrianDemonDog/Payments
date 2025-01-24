namespace Payments.DTOs
{
    public class OrgDto
    {
        public class CreateOrgDto
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string? ParentId { get; set; }
            public string? ParentName { get; set; }
            public string? Region { get; set; }
            public string? CostPlace { get; set; }
        }

        public class UpdateOrgDto
        {
            public string? Name { get; set; }
            public string? Type { get; set; }
            public string? ParentId { get; set; }
            public string? ParentName { get; set; }
            public string? Region { get; set; }
            public string? CostPlace { get; set; }
        }

    }

}

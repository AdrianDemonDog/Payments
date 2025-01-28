using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Payments.Apps.Org.Models
{
    public class OrgModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("VisualId")]
        public int VisualId { get; set; } = 1;

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("TaxNumber")]
        public string? TaxNumber { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("CreatedBy")]
        public string? CreatedBy { get; set; }

        [BsonElement("Created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnoreIfNull]
        [BsonElement("Instance")]
        public string? Instance { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("KYC")]
        public string? KYC { get; set; }

        [BsonElement("KYCStatusChanged")]
        public DateTime? KYCStatusChanged { get; set; }

        [BsonElement("LegalName")]
        public string? LegalName { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("UpdatedBy")]
        public string? UpdatedBy { get; set; }
    }
}

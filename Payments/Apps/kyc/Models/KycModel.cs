using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Payments.Apps.Org.Models
{
    public class KycModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("CompanyId")]
        [BsonRequired]
        public int CompanyId { get; set; }

        [BsonElement("Status")]
        [BsonRequired]
        public string? Status { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("CompanyData")]
        public string? CompanyData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("SaleData")]
        public string? SaleData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("RealData")]
        public string? RealData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("GroupData")]
        public string? GroupData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("PepData")]
        public string? PepData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("DateData")]
        public string? DateData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("KycData")]
        public string? KycData { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("StatusChanged")]
        public DateTime? StatusChanged { get; set; }
    }
}

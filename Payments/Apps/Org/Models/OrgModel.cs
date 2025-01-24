using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Payments.Apps.AppSystem.Helpers;

namespace Payments.Apps.Org.Models
{
    public class OrgModel: IAuditable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("VisualId")]
        public int VisualId { get; set; } = 1;

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("ParentId")]
        public string? ParentId { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("ParentName")]
        public string? ParentName { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Region")]
        public string? Region { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("CostPlace")]
        public string? CostPlace { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnoreIfNull]
        [BsonElement("CreatedBy")]
        public string? CreatedBy { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("UpdatedBy")]
        public string? UpdatedBy { get; set; }
    }

}
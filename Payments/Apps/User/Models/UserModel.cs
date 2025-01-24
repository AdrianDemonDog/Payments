using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Payments.Apps.AppSystem.Helpers;

namespace Payments.Apps.User.Models
{
    public class UserModel: IAuditable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("VisualId")]
        public int VisualId { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Phone")]
        public string? Phone { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("Pnr")]
        public string Pnr { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Addresses")]
        public List<AddressModel>? Addresses { get; set; }

        [BsonElement("Roles")]
        public List<string> Roles { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Status")]
        public string? Status { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("LoggedOnLastTime")]
        public DateTime? LoggedOnLastTime { get; set; }

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

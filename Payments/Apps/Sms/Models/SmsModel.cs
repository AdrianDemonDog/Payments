using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Payments.Apps.Sms.Models
{
    public class SmsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Phone")]
        public string Phone { get; set; }

        [BsonElement("Message")]
        [BsonIgnoreIfNull]
        public string? Message { get; set; }

        [BsonElement("CreatedAt")]
        [BsonIgnoreIfNull]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

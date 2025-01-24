using MongoDB.Bson.Serialization.Attributes;

namespace Payments.Apps.User.Models
{
    public class AddressModel
    {
        [BsonIgnoreIfNull]
        [BsonElement("Address")]
        public string? Address { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("AddressType")]
        public string? AddressType { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("City")]
        public string? City { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("PostalCode")]
        public string? PostalCode { get; set; }

        [BsonElement("Selected")]
        public bool? Selected { get; set; } = false;
    }
}

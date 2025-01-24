using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Payments.Apps.User.Models
{
    public class UserOrgRelation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("OrgId")]
        public string OrgId { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("OrgRolesId")]
        public List<string>? OrgRolesId { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Selected")]
        public bool? Selected { get; set; }
    }
}

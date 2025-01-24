using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Payments.Apps.Payments.Checkout.Models
{
    public class CheckeoutModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cid")]
        public string Cid { get; set; }

        [BsonElement("identifier")]
        public string Identifier { get; set; }

        [BsonElement("api_key_id")]
        public int ApiKeyId { get; set; }

        [BsonElement("created_by")]
        [BsonIgnoreIfNull]
        public int? CreatedBy { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("capture")]
        public int Capture { get; set; }

        [BsonElement("checkout_data")]
        public string CheckoutData { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("status_changed")]
        public string StatusChanged { get; set; }

        [BsonElement("balance")]
        public double Balance { get; set; }

        [BsonElement("company_id")]
        public int CompanyId { get; set; }

        [BsonElement("currency_id")]
        public string CurrencyId { get; set; }

        [BsonElement("test_mode")]
        public bool TestMode { get; set; }
    }
}

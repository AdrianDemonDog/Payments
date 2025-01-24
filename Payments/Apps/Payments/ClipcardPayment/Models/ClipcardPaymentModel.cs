using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Payments.Apps.Payments.ClipcardPayment.Models
{
    public class ClipcardPaymentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cid")]
        public string Cid { get; set; }

        [BsonElement("clipcard_id")]
        [BsonIgnoreIfNull]
        public int? ClipcardId { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("balance")]
        public double Balance { get; set; }

        [BsonElement("sent_via")]
        public string SentVia { get; set; }

        [BsonElement("sent_to")]
        public string SentTo { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("status_changed")]
        [BsonIgnoreIfNull]
        public string StatusChanged { get; set; }

        [BsonElement("payment_data")]
        [BsonIgnoreIfNull]
        public string PaymentData { get; set; }

        [BsonElement("company_id")]
        public int CompanyId { get; set; }

        [BsonElement("currency_id")]
        public string CurrencyId { get; set; }

        [BsonElement("test_mode")]
        public bool TestMode { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Payments.Apps.Payments.InvIncoice.Models
{
    public class InvInvoiceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cid")]
        public string Cid { get; set; }

        [BsonElement("invoice_no")]
        public string InvoiceNo { get; set; }

        [BsonElement("currency_id")]
        public string CurrencyId { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("balance")]
        public double Balance { get; set; }

        [BsonElement("invoice_date")]
        [BsonIgnoreIfNull]
        public string InvoiceDate { get; set; }

        [BsonElement("due_date")]
        [BsonIgnoreIfNull]
        public string DueDate { get; set; }

        [BsonElement("invoice_data")]
        [BsonIgnoreIfNull]
        public string InvoiceData { get; set; }

        [BsonElement("invoice_log")]
        [BsonIgnoreIfNull]
        public string InvoiceLog { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("created_at")]
        [BsonIgnoreIfNull]
        public int? CreatedAt { get; set; }

        [BsonElement("updated_at")]
        [BsonIgnoreIfNull]
        public int? UpdatedAt { get; set; }

        [BsonElement("test_mode")]
        public bool TestMode { get; set; }
    }
}

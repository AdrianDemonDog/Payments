using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Payments.Apps.Payments.Invoice.Models
{
    public class InvoiceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // Equivale al campo $id en PHP.

        [BsonElement("cid")]
        public string Cid { get; set; }

        [BsonElement("invoice_no")]
        public string InvoiceNo { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("balance")]
        public double Balance { get; set; }

        [BsonElement("invoice_data")]
        [BsonIgnoreIfNull]
        public string InvoiceData { get; set; }

        [BsonElement("invoice_rows")]
        [BsonIgnoreIfNull]
        public string InvoiceRows { get; set; }

        [BsonElement("payment_data")]
        [BsonIgnoreIfNull]
        public string PaymentData { get; set; }

        [BsonElement("status")]
        [BsonIgnoreIfNull]
        public string Status { get; set; }

        [BsonElement("due_date")]
        [BsonIgnoreIfNull]
        public string DueDate { get; set; }

        [BsonElement("created_at")]
        [BsonIgnoreIfNull]
        public int? CreatedAt { get; set; }

        [BsonElement("created_by")]
        [BsonIgnoreIfNull]
        public int? CreatedBy { get; set; }

        [BsonElement("updated_at")]
        [BsonIgnoreIfNull]
        public int? UpdatedAt { get; set; }

        [BsonElement("updated_by")]
        [BsonIgnoreIfNull]
        public int? UpdatedBy { get; set; }

        [BsonElement("company_id")]
        public int CompanyId { get; set; }

        [BsonElement("currency_id")]
        public string CurrencyId { get; set; }

        [BsonElement("test_mode")]
        public bool TestMode { get; set; }
    }
}

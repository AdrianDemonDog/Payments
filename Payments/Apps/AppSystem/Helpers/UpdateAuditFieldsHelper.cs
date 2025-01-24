using MongoDB.Bson;
using MongoDB.Driver;
using Payments.Apps.User.Models;
using Payments.StaticClasses;

namespace Payments.Apps.AppSystem.Helpers
{
    public class UpdateAuditFieldsHelper
    {
        /*
        public async static Task UpdateAuditFields<T>(string collection, string modelId, string type) where T : IAuditable
        {
            UserModel? user = AuthenticationHelper.GetUserByJwt();
            var modelCollection = DBCollections.database.GetCollection<T>(collection);
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(modelId));
            if (type == "create")
            {
                if (user != null)
                {
                    var created = Builders<T>.Update.Set(x => x.CreatedBy, user.Id);
                    await modelCollection.UpdateOneAsync(filter, created);
                }
            }
            if (type == "update")
            {
                if (user != null)
                {
                    var updated = Builders<T>.Update.Set(x => x.UpdatedBy, user.Id);
                    await modelCollection.UpdateOneAsync(filter, updated);
                }
                var updatedTime = Builders<T>.Update.Set(x => x.UpdatedAt, DateTime.UtcNow);
                await modelCollection.UpdateOneAsync(filter, updatedTime);
            }
        }
        */
    }

    public interface IAuditable
    {
        DateTime? UpdatedAt { get; set; }

        string? UpdatedBy { get; set; }

        DateTime? CreatedAt { get; set; }

        string? CreatedBy { get; set; }
    }
}

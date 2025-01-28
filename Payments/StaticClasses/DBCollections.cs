using MongoDB.Driver;
using Payments.Apps.AppSystem.Helpers;
using Payments.Apps.User.Models;

namespace Payments.StaticClasses
{
    public class DBCollections
    {
        // private static dynamic connectionString = UtilityHelper.CheckLocal() == false ? UtilityHelper.GetMongoDbConnection() : "mongodb://localhost:27017";

        // private static string databaseName = UtilityHelper.CheckLocal() == false ? UtilityHelper.GetAppSetting("DatabaseName") : "PaymentsDB";


        private static string databaseName = "PaymentsDB";
        private static string connectionString = "mongodb://localhost:27017";
        private static MongoClient client = new MongoClient(connectionString);
        public static IMongoDatabase database = client.GetDatabase(databaseName);

        // COLLECTIONS
        public static IMongoCollection<UserModel> userCollection = database.GetCollection<UserModel>("Users");
        public static IMongoCollection<UserModel> orgsCollection = database.GetCollection<UserModel>("company");
        public static IMongoCollection<UserModel> kycCollection = database.GetCollection<UserModel>("kyc");
    }
}

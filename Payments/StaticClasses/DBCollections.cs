using MongoDB.Driver;
using Payments.Apps.AppSystem.Helpers;
using Payments.Apps.Sms.Models;
using Payments.Apps.User.Models;

namespace Payments.StaticClasses
{
    public class DBCollections
    {
        // private static dynamic connectionString = UtilityHelper.CheckLocal() == false ? UtilityHelper.GetMongoDbConnection() : "mongodb://localhost:27017";

        // private static string databaseName = UtilityHelper.CheckLocal() == false ? UtilityHelper.GetAppSetting("DatabaseName") : "PaymentsDB";


        private static string databaseName = UtilityHelper.GetAppSetting("MongoDB:DatabaseName");
        private static string connectionString = UtilityHelper.GetAppSetting("MongoDB:ConnectionString");
        private static MongoClient client = new MongoClient(connectionString);
        public static IMongoDatabase database = client.GetDatabase(databaseName);

        // COLLECTIONS
        public static IMongoCollection<UserModel> userCollection = database.GetCollection<UserModel>("Users");
        public static IMongoCollection<UserModel> orgsCollection = database.GetCollection<UserModel>("company");
        public static IMongoCollection<UserModel> kycCollection = database.GetCollection<UserModel>("kyc");
        public static IMongoCollection<SmsModel> smsCollection = database.GetCollection<SmsModel>("Sms");
    }
}

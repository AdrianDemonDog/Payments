using MongoDB.Driver;

namespace Payments.Apps.AppSystem.Helpers
{
    public class UtilityHelper
    {
        // GET APPSETTINGS / CUSTOMSETTINGS VALUE
        /*
        public static string GetAppSetting(string value)
        {
            if (ApplicationValues.secretValues.ContainsKey(value))
            {
                return ApplicationValues.secretValues.GetValueOrDefault(value, "");
            }
            return "";
        }
        */

        /*
        public static MongoClientSettings GetMongoDbConnection()
        {
            var settings = MongoClientSettings.FromConnectionString(GetAppSetting("DBConnect"));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            return settings;
        }
        */

        // CHECK LOCAL
        public static bool CheckLocal()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            if (configuration["Local"] != null)
            {
                return true;
            }
            return false;
        }
    }
}

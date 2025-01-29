using MongoDB.Driver;
using Payments.Apps.AppSystem.Helpers;
using Payments.Apps.Sms.Models;
using Payments.StaticClasses;
using System.Web;

namespace Payments.Apps.Sms.Helpers
{
    public class SmsHelper
    {
        public static async Task<string> SendSMS(string phone, string message)
        {
            string? _apiKey = UtilityHelper.GetAppSetting("TextLocal:ApiKey");
            string? _sender = UtilityHelper.GetAppSetting("TextLocal:SenderName");
            string _postUrl = UtilityHelper.GetAppSetting("TextLocal:ApiUrl");

            if (_apiKey != null && _sender != null && _postUrl != null)
            {
                string messageEncoded = HttpUtility.UrlEncode(message);
                string phoneCleaned = phone.Trim(new char[] { '+' });
                using (var client = new HttpClient())
                {
                    var body = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("apikey", _apiKey),
                        new KeyValuePair<string, string>("numbers", phoneCleaned),
                        new KeyValuePair<string, string>("message", messageEncoded),
                        new KeyValuePair<string, string>("sender", _sender)
                    };

                    var response = await client.PostAsync(_postUrl, new FormUrlEncodedContent(body));
                    await response.Content.ReadAsStringAsync();

                    var smsModel = new SmsModel
                    {
                        Phone = phone,
                        Message = message
                    };

                    DBCollections.smsCollection.InsertOne(smsModel);
                    return "SMS sent successfully.";
                }
            }
            return "Error: SMS not sent.";
        }

        public static List<SmsModel> GetSmsList() => DBCollections.smsCollection.Find(_ => true).ToList();
    }
}

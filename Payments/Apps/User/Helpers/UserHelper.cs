using MongoDB.Driver;
using Payments.Apps.User.Models;
using Payments.StaticClasses;

namespace Payments.Apps.User.Helpers
{
    public class UserHelper
    {
        public static UserModel GetUserById(string id) => DBCollections.userCollection.Find(u => u.Id == id).FirstOrDefault();
        public static UserModel GetUserByEmail(string email) => DBCollections.userCollection.Find(u => u.Email == email).FirstOrDefault();
    }
}
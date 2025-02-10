namespace Payments.Common.Events
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}

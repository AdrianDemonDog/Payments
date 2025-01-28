using Payments.Apps.Mail.Services;

namespace Payments.Apps.Mail.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, EmailType emailType, string? token = null);
    }
}

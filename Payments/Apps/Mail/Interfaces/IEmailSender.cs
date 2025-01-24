namespace Payments.Apps.Mail.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string subject);
    }
}

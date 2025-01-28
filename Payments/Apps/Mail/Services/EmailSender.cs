using System.Net.Mail;
using System.Net;
using System.Text;
using Payments.Apps.Mail.Interfaces;

namespace Payments.Apps.Mail.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _senderEmail = "adrianmfer99@gmail.com";
        private readonly string _senderPassword = "fbhxpviizbkrbvou";

        public void SendEmail(string toEmail, EmailType emailType, string? token = null)
        {
            // Configurar SMTP
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword)
            };

            // Obtener el contenido del correo basado en el tipo
            var (subject, body) = GetEmailTemplate(emailType, token);

            // Configurar el mensaje
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail),
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };

            mailMessage.To.Add(toEmail);

            // Enviar el correo
            client.Send(mailMessage);
        }

        private (string subject, string body) GetEmailTemplate(EmailType emailType, string? token)
        {
            switch (emailType)
            {
                case EmailType.Verification:
                    return (
                        "Welcome to Our Platform!",
                        $@"
                        <html>
                        <body>
                            <h1>Welcome!</h1>
                            <p>Thank you for registering on our platform. You are now verified!</p>
                            <p>Please log in to explore more.</p>
                        </body>
                        </html>"
                    );

                case EmailType.Registration:
                    return (
                        "Verify Your Email Address",
                        $@"
                        <html>
                        <body>
                            <h1>Verify Your Email</h1>
                            <p>Thank you for signing up! Please verify your email using the token below:</p>
                            <p><strong>{token}</strong></p>
                            <p>Or click the button below to verify:</p>
                            <a href='#' style='background-color:#593068;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>Verify Email</a>
                        </body>
                        </html>"
                    );

                case EmailType.PasswordReset:
                    return (
                        "Reset Your Password",
                        $@"
                        <html>
                        <body>
                            <h1>Password Reset</h1>
                            <p>We received a request to reset your password. Use the token below to reset it:</p>
                            <p><strong>{token}</strong></p>
                            <p>If you did not request this, please ignore this email.</p>
                        </body>
                        </html>"
                    );

                default:
                    throw new ArgumentOutOfRangeException(nameof(emailType), "Unsupported email type.");
            }
        }
    }

    public enum EmailType
    {
        Registration,
        Verification,
        PasswordReset
    }
}

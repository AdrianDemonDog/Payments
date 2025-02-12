using DemonDog.Contracts.Models;
using MassTransit;
using System.Net.Mail;
using System.Net;

namespace Payments.Common.Events
{
    public class ForgotPasswordConsumer : IConsumer<ForgotPasswordEvent>
    {
        private readonly string _senderEmail = "adrianmfer99@gmail.com";
        private readonly string _senderPassword = "cawqmfhygyjoviww";

        public async Task Consume(ConsumeContext<ForgotPasswordEvent> context)
        {
            var message = context.Message;
            Console.WriteLine($"Recibido evento para {message.Email}");

            // ✅ Validar si el email es nulo o vacío antes de intentar enviar el correo
            if (string.IsNullOrWhiteSpace(message.Email))
            {
                Console.WriteLine("⚠️ Error: El email recibido está vacío. No se puede enviar el correo.");
                return;
            }

            try
            {
                using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword)
                };

                // ✅ Crear el enlace de restablecimiento de contraseña
                string resetLink = $"https://demon-dog.com/reset-password?token={message.Token}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Reset Your Password - DemonDog",
                    Body = $@"
                            Hello,

                            We received a request to reset your password for your DemonDog account. Click the link below to reset your password:

                            🔹 {resetLink}

                            If you did not request this, please ignore this email.

                            Best,
                            DemonDog Team",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(message.Email);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"✅ Email de recuperación enviado correctamente a {message.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando email: {ex.Message}");
            }
        }
    }
}

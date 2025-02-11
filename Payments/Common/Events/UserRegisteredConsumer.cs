using demonDog.IdentityService.Events;
using MassTransit;
using Payments.Apps.AppSystem.Helpers;
using System.Net;
using System.Net.Mail;

namespace Payments.Common.Events
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {

        private readonly string _senderEmail = "adrianmfer99@gmail.com";
        private readonly string _senderPassword = "cawqmfhygyjoviww";

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var message = context.Message;
            Console.WriteLine($"Recibido evento para {message.Email}");

            try
            {
                using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("adrianmfer99@gmail.com"),
                    Subject = "Welcome to DemonDog!",
                    Body = $"Hello {message.Email},\n\nWelcome to DemonDog! Your account has been successfully registered.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(message.Email);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {message.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
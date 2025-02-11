using demonDog.IdentityService.Events;
using MassTransit;

namespace Payments.Common.Events
{
    public class UserLoguedConsumer : IConsumer<UserLoguedEvent>
    {
        public Task Consume(ConsumeContext<UserLoguedEvent> context)
        {
            Console.WriteLine($"Recibido evento de usuario logueado con token {context.Message.Token}");
            return Task.CompletedTask;
        }
    }
}

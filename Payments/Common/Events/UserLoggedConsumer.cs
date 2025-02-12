using DemonDog.Contracts.Models;
using MassTransit;

namespace Payments.Common.Events
{
    public class UserLoggedConsumer : IConsumer<UserLoggedEvent>
    {
        public Task Consume(ConsumeContext<UserLoggedEvent> context)
        {
            Console.WriteLine($"Received event from logged in user with token {context.Message.Token}");
            return Task.CompletedTask;
        }
    }
}

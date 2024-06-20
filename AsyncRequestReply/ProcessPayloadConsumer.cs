using MessageBus;
using System.Text;

namespace AsyncRequestReply
{
    public class ProcessPayloadConsumer : IConsumer<ProcessPayload>
    {
        public async Task Consume(IConsumerContext<ProcessPayload> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Consuming ProcessPayload");

            var textBytes = Encoding.UTF8.GetBytes(context.Message.RequestPayload);
            var base64String = Convert.ToBase64String(textBytes);


            //Simulate processing time to ensure the duration is around 100ms or more
            await Task.Delay(100);

            //This will publish a message / event to the output queue
            await context.Respond<ProcessPayloadResult>(new(base64String), cancellationToken);
        }
    }
}

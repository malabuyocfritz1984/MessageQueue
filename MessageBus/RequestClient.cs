using System.Threading.Channels;

namespace MessageBus
{
    public class RequestClient<TRequest, TResponse> : IRequestClient<TRequest, TResponse>
        where TResponse : class
        where TRequest : class
    {
        private readonly ChannelReader<TResponse> _outputQueue;
        private readonly IPublisher<TRequest> _publisher;

        public RequestClient(ChannelReader<TResponse> outputQueue, IPublisher<TRequest> publisher)
        {
            _outputQueue = outputQueue;
            _publisher = publisher;
        }

        public async Task<TResponse> Get(TRequest request, CancellationToken cancellationToken = default)
        {
            await _publisher.Publish(request, cancellationToken).ConfigureAwait(false);

            //Consume messages from output queue, this will make the request WAIT for the result
            var response = await _outputQueue.ReadAsync(cancellationToken);
            return response;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}

using System.Threading.Channels;

namespace MessageBus
{
    public class ConsumerContext<T, TResponse> : IConsumerContext<T> where T : class
    {
        private readonly ChannelWriter<TResponse> _outputQueue;
        public T Message { get; }

        public ConsumerContext(ChannelWriter<TResponse> outputQueue, T message)
        {
            _outputQueue = outputQueue;
            Message = message;
        }

        public async Task Respond<T1>(T1 message, CancellationToken cancellationToken = default) where T1 : class
        {
            //Publish message or event to output queue
            var m = (TResponse)Convert.ChangeType(message, typeof(TResponse));
            await _outputQueue.WriteAsync(m, cancellationToken).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            _outputQueue.TryComplete();
            return ValueTask.CompletedTask;
        }
    }
}

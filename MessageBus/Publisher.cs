using System.Threading.Channels;

namespace MessageBus
{
    public class Publisher<T> : IPublisher<T> where T : class
    {
        private readonly ChannelWriter<T> _queue;

        public Publisher(ChannelWriter<T> queue)
        {
            _queue = queue;
        }

        public async Task Publish(T @event, CancellationToken cancellationToken = default)
        {
            await _queue.WriteAsync(@event, cancellationToken).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            _queue.TryComplete();
            return ValueTask.CompletedTask;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace MessageBus
{
    public class MessageBus<T, TResponse> : IMessageBus<T, TResponse> 
        where T : class
        where TResponse : class
    {
        private readonly ChannelReader<T> _queue;
        private readonly ChannelWriter<TResponse> _outputQueue;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageBus(ChannelReader<T> queue, ChannelWriter<TResponse> outputQueue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _outputQueue = outputQueue;
            _scopeFactory = scopeFactory;
        }

        public async Task Start(CancellationToken cancellationToken = default) 
        {
            //ensure cancellationtoken is created
            EnsureCancellationTokenSourceIsCreated(cancellationToken);

            await using var scope = _scopeFactory.CreateAsyncScope();

            //retrieve all the event consumers
            var consumers = scope.ServiceProvider.GetServices<IConsumer<T>>().ToList();

            if (!consumers.Any()) 
            {
                return;
            }

            //When there is consumer then trigger it
            await TriggerConsumers(consumers).ConfigureAwait(false);
        }

        private void EnsureCancellationTokenSourceIsCreated(CancellationToken cancellationToken = default) 
        {
            if (_cancellationTokenSource is not null && !_cancellationTokenSource.IsCancellationRequested) 
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = cancellationToken.CanBeCanceled ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken) : new CancellationTokenSource();
        }

        private async Task TriggerConsumers(List<IConsumer<T>> consumers) 
        {
            var queueIterator = _queue.ReadAllAsync(_cancellationTokenSource!.Token)
                .WithCancellation(_cancellationTokenSource.Token)
                .ConfigureAwait(false);

                await foreach (var @event in queueIterator)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    //trigger all consumers
                    await Parallel.ForEachAsync(consumers, _cancellationTokenSource.Token,
                        async (consumer, token) => await consumer.Consume(new ConsumerContext<T, TResponse>(_outputQueue, @event), token)
                        .ConfigureAwait(false)
                    ).ConfigureAwait(false);
                }
        }

        public async Task Stop(CancellationToken cancellationToken = default)
        {
            await DisposeAsync().ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            _cancellationTokenSource?.Cancel();
            return ValueTask.CompletedTask;
        }
    }
}

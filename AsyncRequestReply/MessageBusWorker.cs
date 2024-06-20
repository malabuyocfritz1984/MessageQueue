using MessageBus;

namespace AsyncRequestReply
{
    public class MessageBusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IEnumerable<IMessageBus>? _messageBuses;

        public MessageBusWorker(IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            await StartMessageBuses(scope.ServiceProvider, stoppingToken);
        }

        private async Task StartMessageBuses(IServiceProvider sp, CancellationToken cancellationToken)
        {
            _messageBuses = sp.GetServices<IMessageBus>();

            foreach (var bus in _messageBuses)
            {
                await bus.Start(cancellationToken).ConfigureAwait(false);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            foreach (var bus in _messageBuses!)
            {
                await bus.Stop(stoppingToken);
            }

            await base.StopAsync(stoppingToken);
        }
    }
}

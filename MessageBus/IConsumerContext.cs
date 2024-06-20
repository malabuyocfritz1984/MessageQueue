namespace MessageBus
{
    public interface IConsumerContext : IAsyncDisposable
    {
        Task Respond<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }

    public interface IConsumerContext<T> : IConsumerContext where T : class
    {
        T Message { get; }
    }
}

namespace MessageBus
{
    public interface IPublisher<T> : IAsyncDisposable where T : class
    {
        Task Publish(T @event, CancellationToken cancellationToken = default);
    }
}

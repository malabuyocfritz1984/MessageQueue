namespace MessageBus
{
    public interface IMessageBus : IAsyncDisposable 
    {
        Task Start(CancellationToken cancellationToken = default);
        Task Stop(CancellationToken cancellationToken = default);
    }

    public interface IMessageBus<T, TResponse> : IMessageBus 
        where T : class
        where TResponse : class
    {
    }
}

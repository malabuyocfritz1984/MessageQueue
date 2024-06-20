namespace MessageBus
{
    public interface IRequestClient<TRequest, TResponse> : IAsyncDisposable 
        where TResponse : class
        where TRequest : class
    {
        Task<TResponse> Get(TRequest request, CancellationToken cancellationToken = default);
    }
}

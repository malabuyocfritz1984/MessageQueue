namespace MessageBus
{
    public interface IConsumer<T> where T : class
    {
        Task Consume(IConsumerContext<T> context, CancellationToken cancellationToken = default);
    }
}

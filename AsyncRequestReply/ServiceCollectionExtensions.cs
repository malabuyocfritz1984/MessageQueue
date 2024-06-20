using MessageBus;
using System.Threading.Channels;

namespace AsyncRequestReply
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryMessageBus<T, TResponse, TConsumer>(this IServiceCollection services)
            where T : class
            where TResponse : class
            where TConsumer : class, IConsumer<T>
        {
            var inputQueue = Channel.CreateUnbounded<T>(
                    new UnboundedChannelOptions
                    {
                        AllowSynchronousContinuations = false
                    }
                );

            var outputQueue = Channel.CreateUnbounded<TResponse>(
                    new UnboundedChannelOptions
                    {
                        AllowSynchronousContinuations = false
                    }
                );

            services.AddScoped<IConsumer<T>, TConsumer>();

            // Inject publisher and requestclient
            services.AddSingleton<IPublisher<T>>(new Publisher<T>(inputQueue.Writer));
            var requestClientFactory = (IServiceProvider provider) => new RequestClient<T, TResponse>(outputQueue.Reader, provider.GetRequiredService<IPublisher<T>>());
            services.AddSingleton<IRequestClient<T, TResponse>>(requestClientFactory.Invoke);

            var messageBusFactory = (IServiceProvider provider) => new MessageBus<T, TResponse>(
                inputQueue.Reader,
                outputQueue.Writer,
                provider.GetRequiredService<IServiceScopeFactory>()
                );

            services.AddSingleton<IMessageBus>(messageBusFactory.Invoke);
            services.AddSingleton<IMessageBus<T, TResponse>>(messageBusFactory.Invoke);

            return services;
        }
    }
}

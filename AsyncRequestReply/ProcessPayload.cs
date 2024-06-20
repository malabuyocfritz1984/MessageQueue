using MessageBus;

namespace AsyncRequestReply
{
    public record ProcessPayload(string RequestPayload) : Event;
}

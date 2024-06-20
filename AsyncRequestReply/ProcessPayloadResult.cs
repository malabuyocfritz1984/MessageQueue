using MessageBus;

namespace AsyncRequestReply
{
    public record ProcessPayloadResult(string ResponsePayload) : Event;
}

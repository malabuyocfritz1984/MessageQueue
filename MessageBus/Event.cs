namespace MessageBus
{
    public abstract record Event
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
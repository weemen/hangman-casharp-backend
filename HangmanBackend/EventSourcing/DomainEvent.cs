namespace HangmanBackend.Domain
{
    public interface DomainEvent
    {
        string serialize(DomainEvent domainEvent);
        DomainEvent deserialize(string json);
    }
}
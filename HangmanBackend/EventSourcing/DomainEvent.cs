namespace HangmanBackend.Domain
{
    public interface DomainEvent
    {
        string Serialize(DomainEvent domainEvent);
    }
}
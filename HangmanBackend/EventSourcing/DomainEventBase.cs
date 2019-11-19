using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class DomainEventBase: DomainEvent
    {
        public string serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public DomainEvent deserialize(string json)
        {
            return JsonConvert.DeserializeObject<GameStarted>(json);
        }
    }
}
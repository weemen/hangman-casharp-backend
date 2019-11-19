using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class GameLost : DomainEvent
    {
        private string reason;
        
        public GameLost(string reason)
        {
            this.reason = reason;
        }

        public string Reason => reason;
        
        public string serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public DomainEvent deserialize(string json)
        {
            return JsonConvert.DeserializeObject<GameLost>(json);
        }
    }
}
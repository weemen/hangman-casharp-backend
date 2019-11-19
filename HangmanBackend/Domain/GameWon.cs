using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class GameWon : DomainEvent
    {
        private string reason;
        
        public GameWon(string reason)
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
            return JsonConvert.DeserializeObject<GameWon>(json);
        }
    }
}
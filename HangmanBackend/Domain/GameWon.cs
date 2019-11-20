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
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameWon Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new GameWon((string) obj.Reason);
        }
    }
}
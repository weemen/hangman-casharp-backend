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
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameLost Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new GameLost((string) obj.Reason);
        }
    }
}
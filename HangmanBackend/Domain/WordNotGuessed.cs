using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class WordNotGuessed : DomainEvent
    {
        private string word;
        
        public WordNotGuessed(string word)
        {
            this.word = word;
        }

        public string Word => word;
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static WordNotGuessed Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new WordNotGuessed((string) obj.Word);
        }
    }
}
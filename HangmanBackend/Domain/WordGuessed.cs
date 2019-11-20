using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class WordGuessed : DomainEvent
    {
        private string word;
        
        public WordGuessed(string word)
        {
            this.word = word;
        }

        public string Word => word;
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static WordGuessed Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new WordGuessed((string) obj.Word);
        }
    }
}
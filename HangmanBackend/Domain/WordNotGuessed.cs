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
        
        public string serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public DomainEvent deserialize(string json)
        {
            return JsonConvert.DeserializeObject<WordGuessed>(json);
        }
    }
}
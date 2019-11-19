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
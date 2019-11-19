using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class LetterGuessed: DomainEvent
    {
        private char letter;
        
        public LetterGuessed(char letter)
        {
            this.letter = letter;
        }

        public char Letter => letter;
        
        public string serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public DomainEvent deserialize(string json)
        {
            return JsonConvert.DeserializeObject<LetterGuessed>(json);
        }
    }
}
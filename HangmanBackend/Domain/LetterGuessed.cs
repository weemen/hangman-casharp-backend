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
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static LetterGuessed Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new LetterGuessed((char) obj.Letter);
        }
    }
}
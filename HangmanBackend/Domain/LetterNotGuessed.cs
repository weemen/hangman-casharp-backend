using System;
using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public class LetterNotGuessed: DomainEvent
    {
        private char letter;
        
        public LetterNotGuessed(char letter)
        {
            this.letter = letter;
        }

        public char Letter => letter;
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static LetterNotGuessed Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new LetterNotGuessed((char) obj.Letter);
        }
    }
}
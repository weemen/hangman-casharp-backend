using System;
using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    
    public class GameStarted : DomainEvent
    {
        private string accountId;
        private string gameId;
        private string word;
        private int level;

        public GameStarted(string accountId, string gameId, string word, int level)
        {
            this.accountId = accountId;
            this.gameId = gameId;
            this.word = word;
            this.level = level;
        }

        public string AccountId => accountId;

        public string GameId => gameId;

        public string Word => word;

        public int Level => level;
        
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameStarted Deserialize(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            return new GameStarted((string) obj.AccountId, (string) obj.GameId, (string) obj.Word, (int) obj.Level);
        }
    }
}
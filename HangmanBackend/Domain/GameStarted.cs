using System;
using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    
    public class GameStarted : DomainEventBase
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
    }
}
using System;
using HangmanBackend.Domain;

namespace HangmanBackend.Application
{
    public class StartGame
    {
        private Guid gameId;
        private Guid accountId;
        private string word;
        private DifficultySetting level;
        
        public StartGame(Guid gameId, Guid accountId, string word, DifficultySetting level)
        {
            this.gameId = gameId;
            this.accountId = accountId;
            this.word = word;
            this.level = level;
        }

        public Guid GameId => gameId;

        public Guid AccountId => accountId;

        public string Word => word;

        public DifficultySetting Level => level;
    }
}
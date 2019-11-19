using System;

namespace HangmanBackend.Application
{
    public class GuessWord
    {
        private Guid gameId;
        private string word;
        
        public GuessWord(Guid gameId, string word)
        {
            this.gameId = gameId;
            this.word = word;
        }

        public Guid GameId => gameId;

        public string Word => word;
    }
}
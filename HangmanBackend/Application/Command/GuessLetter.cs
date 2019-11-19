using System;

namespace HangmanBackend.Application
{
    public class GuessLetter
    {
        private Guid gameId;
        private char letter;
        
        public GuessLetter(Guid gameId, char letter)
        {
            this.gameId = gameId;
            this.letter = letter;
        }

        public Guid GameId => gameId;

        public char Letter => letter;
    }
}
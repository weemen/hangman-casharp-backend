using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using HangmanBackend.Application;
using Newtonsoft.Json;

namespace HangmanBackend.Domain
{
    public enum DifficultySetting
    {
        EASY,
        MEDIUM,
        HARD
    }

    public class Game : AggregateRoot
    {
        //should be in a base class
        protected List<EventData> uncommittedEvents = new List<EventData>();
        protected int playHead;
        
        private string accountId;
        private string gameId;
        private string word;
        private int level;
        
        private bool GameOver;
        private int triesRemaining;
        private string gameEndReason;
        private char[] guessedLetters;
        private char[] missGuessedLetters;

        public Guid getAggregateRootId()
        {
            return new Guid(this.gameId);
        }

        public List<EventData> getUncommittedEvents()
        {
            return uncommittedEvents;
        }

        public int getPlayHead()
        {
            return this.playHead;
        }

        private void Record(DomainEvent gameEvent, string streamId = "")
        {
            
            this.uncommittedEvents.Add(new EventData(
                Guid.NewGuid(), 
                gameEvent.GetType().FullName, 
                true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(gameEvent)),
                Encoding.UTF8.GetBytes("{}")));
            
            this.Apply((dynamic) gameEvent);
        }
        
        public static Game StartGame(Guid accountId, Guid gameId, string word, DifficultySetting level)
        {
            var game = new Game();
            game.Record(new GameStarted(accountId.ToString(), gameId.ToString(), word, (int) level), gameId.ToString());
            return game;
        }
        
        private void Apply(GameStarted gameEvent)
        {
            this.gameId = gameEvent.GameId;
            this.accountId = gameEvent.AccountId;
            this.word = gameEvent.Word;
            this.level = gameEvent.Level;
            
            this.triesRemaining = 10;
            this.GameOver = false;
        }

        public void guessLetter(GuessLetter command)
        {

            // letter guessed and all letter guessed here
            if (this.word.Contains(command.Letter) && this.hasAllLettersGuessed(command.Letter))
            {
                this.Record(new LetterGuessed(command.Letter));
                this.Record(new GameWon("All letters guessed"));
            } 
            else if (this.word.Contains(command.Letter))
            {
                this.Record(new LetterGuessed(command.Letter));
            }
            else
            {
                this.Record(new LetterNotGuessed(command.Letter));
                if ((this.triesRemaining - 1) < 0)
                {
                    this.Record(new GameLost("No tries remaining"));
                }
            }
        }

        private bool hasAllLettersGuessed(char letter)
        {
            var charactersInWord = this.word.ToCharArray();
            var guessedLetters = (char[]) this.guessedLetters.Clone();
            guessedLetters.Append(letter);
            return !charactersInWord.Except(guessedLetters).Any();
        }
        
        private void Apply(LetterGuessed gameEvent)
        {
            this.guessedLetters.Append(gameEvent.Letter);
        }
        
        private void Apply(LetterNotGuessed gameEvent)
        {
            // reduce tries
            this.triesRemaining--;
            this.missGuessedLetters.Append(gameEvent.Letter);
        }

        public void guessWord(GuessWord command)
        {
            if (command.Word == this.word)
            {
                this.Record(new WordGuessed(command.Word));
                this.Record(new GameWon("Word guessed"));
            }
            else
            {
                this.Record(new WordNotGuessed(command.Word));
                this.Record(new GameLost("Word not guessed"));
            }
        }
        
        private void Apply(WordGuessed gameEvent)
        {
            this.triesRemaining--;
            this.GameOver = true;
        }

        private void Apply(WordNotGuessed gameEvent)
        {
            this.triesRemaining--;
            this.GameOver = true;
        }
        
        private void Apply(GameWon gameEvent)
        {
            this.GameOver = true;
            this.gameEndReason = gameEvent.Reason;
        }

        private void Apply(GameLost gameEvent)
        {
            this.GameOver = true;
            this.gameEndReason = gameEvent.Reason;
        }
    }
}
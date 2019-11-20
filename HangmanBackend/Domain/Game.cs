using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using HangmanBackend.Application;
using HangmanBackend.Exceptions;
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
        protected int playHead = -1;
        
        private string accountId;
        private string gameId;
        private string word;
        private int level;
        
        private bool GameOver;
        private int triesRemaining;
        private string gameEndReason;
        private List<char> guessedLetters;
        private List<char> missGuessedLetters;

        public Guid getAggregateRootId()
        {
            return new Guid(this.gameId);
        }

        public List<EventData> getUncommittedEvents()
        {
            return uncommittedEvents;
        }

        public void setExpectedPlayHead(int expectedPlayHead)
        {
            this.playHead = expectedPlayHead;
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
            this.playHead++;
        }
        
        public static Game StartGame(Guid accountId, Guid gameId, string word, DifficultySetting level)
        {
            var game = new Game();
            game.Record(new GameStarted(accountId.ToString(), gameId.ToString(), word, (int) level), gameId.ToString());
            return game;
        }
        
        public void Apply(GameStarted gameEvent)
        {
            this.guessedLetters = new List<char>();
            this.missGuessedLetters = new List<char>();
            
            this.gameId = gameEvent.GameId;
            this.accountId = gameEvent.AccountId;
            this.word = gameEvent.Word;
            this.level = gameEvent.Level;
            
            this.triesRemaining = 10;
            this.GameOver = false;
        }

        public void guessLetter(GuessLetter command)
        {
            if (this.GameOver)
            {
                throw new DomainException("Game is already over");
            }
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
            var guessedLetters = new List<char>(this.guessedLetters);
            guessedLetters.Append(letter);
            return !charactersInWord.Except(guessedLetters).Any();
        }
        
        public void Apply(LetterGuessed gameEvent)
        {
            this.guessedLetters.Append(gameEvent.Letter);
        }
        
        public void Apply(LetterNotGuessed gameEvent)
        {
            // reduce tries
            this.triesRemaining--;
            this.missGuessedLetters.Append(gameEvent.Letter);
        }

        public void guessWord(GuessWord command)
        {
            if (this.GameOver)
            {
                throw new DomainException("Game is already over");
            }
            
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
        
        public void Apply(WordGuessed gameEvent)
        {
            this.triesRemaining--;
            this.GameOver = true;
        }

        public void Apply(WordNotGuessed gameEvent)
        {
            this.triesRemaining--;
            this.GameOver = true;
        }
        
        public void Apply(GameWon gameEvent)
        {
            this.GameOver = true;
            this.gameEndReason = gameEvent.Reason;
        }

        public void Apply(GameLost gameEvent)
        {
            this.GameOver = true;
            this.gameEndReason = gameEvent.Reason;
        }
    }
}
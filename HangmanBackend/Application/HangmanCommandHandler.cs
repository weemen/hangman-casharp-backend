using System;
using HangmanBackend.Domain;
using HangmanBackend.Exceptions;
using HangmanBackend.Infrastructure;

namespace HangmanBackend.Application
{
    public class HangmanCommandHandler
    {
        private EventStoreRepository<Game> repository;
        
        public HangmanCommandHandler(EventStoreRepository<Game> repository)
        {
            this.repository = repository;
        }
        public void handleStartGame(StartGame command)
        {
            repository.Save(Game.StartGame(command.AccountId, command.GameId, command.Word, command.Level));
        }

        public void handleGuessLetter(GuessLetter command, int expectedVersion)
        {
            //load aggregate and fire cmd
            var aggregate = this.repository.Load(command.GameId) as Game; 
            aggregate.setExpectedPlayHead(expectedVersion);
            aggregate.guessLetter(command);
            repository.Save(aggregate);

         }

        public void handleGuessWord(GuessWord command, int expectedPlayHead)
        {
            //load aggregate and fire cmd
            var aggregate = this.repository.Load(command.GameId) as Game;
            aggregate.setExpectedPlayHead(expectedPlayHead);
            aggregate.guessWord(command);
            repository.Save(aggregate);
        }
    }
}
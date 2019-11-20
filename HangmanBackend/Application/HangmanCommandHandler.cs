using System;
using HangmanBackend.Domain;
using HangmanBackend.Exceptions;
using HangmanBackend.Infrastructure;

namespace HangmanBackend.Application
{
    public class HangmanCommandHandler
    {
        private EventStoreRepository repository;
        
        public HangmanCommandHandler(EventStoreRepository repository)
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
            try
            {
                var aggregate = this.repository.Load(command.GameId) as Game;
                aggregate.setExpectedPlayHead(expectedVersion);
                aggregate.guessLetter(command);
                this.repository.Save(aggregate);
            } catch (DomainException ex)
            {
                Console.WriteLine("Domain Exception Caught");
            }
            
        }

        public void handleGuessWord(GuessWord command, int expectedPlayHead)
        {
            //load aggregate and fire cmd
            var aggregate = this.repository.Load(command.GameId) as Game;
            aggregate.setExpectedPlayHead(expectedPlayHead);
            aggregate.guessWord(command);
            this.repository.Save(aggregate);
        }
    }
}
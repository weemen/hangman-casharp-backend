using HangmanBackend.Domain;
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

        public void handleGuessLetter(GuessLetter command)
        {
            //load aggregate and fire cmd
            AggregateRoot aggregate = this.repository.Load(command.GameId);
            ((Game)aggregate).guessLetter(command);
            this.repository.Save(aggregate);
        }

        public void handleGuessWord(GuessWord command)
        {
            //load aggregate and fire cmd
            AggregateRoot aggregate = this.repository.Load(command.GameId);
            ((Game)aggregate).guessWord(command);
            this.repository.Save(aggregate);
        }
    }
}
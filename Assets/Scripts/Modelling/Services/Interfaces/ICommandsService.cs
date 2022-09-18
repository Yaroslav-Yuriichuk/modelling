using Modelling.Commands;

namespace Modelling.Services
{
    public interface ICommandsService
    {
        public void AddCommand(ICommand command);
        public void UndoLastCommand();
    }
}
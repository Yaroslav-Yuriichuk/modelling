using Modelling.Commands;

namespace Modelling.Services
{
    public interface IModellingService
    {
        public void AddCommand(ICommand command);
        public void UndoLastCommand();
    }
}
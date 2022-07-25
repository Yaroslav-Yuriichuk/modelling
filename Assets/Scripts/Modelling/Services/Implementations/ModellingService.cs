using System.Collections.Generic;
using Modelling.Commands;

namespace Modelling.Services
{
    public class ModellingService : IModellingService
    {
        private Stack<ICommand> _commands = new Stack<ICommand>();
        private readonly ILogger _logger;
        
        public ModellingService(ILogger logger)
        {
            _logger = logger;
        }
        
        public void AddCommand(ICommand command)
        {
            _logger.Log("Added command");
            _commands.Push(command);
            command.Execute();
        }

        public void UndoLastCommand()
        {
            if (_commands.Count == 0) return;

            _logger.Log("Undo last command");
            ICommand command = _commands.Pop();
            command.Undo();
        }
    }
}
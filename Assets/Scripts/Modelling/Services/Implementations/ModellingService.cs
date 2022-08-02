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
            _commands.Push(command);
            command.Execute();
        }

        public void UndoLastCommand()
        {
            if (_commands.Count == 0) return;

            ICommand command = _commands.Pop();
            command.Undo();
        }
    }
}
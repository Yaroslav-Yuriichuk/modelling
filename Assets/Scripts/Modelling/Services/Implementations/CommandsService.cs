using System.Collections.Generic;
using Modelling.Commands;

namespace Modelling.Services
{
    public sealed class CommandsService : ICommandsService
    {
        private Stack<ICommand> _commands = new Stack<ICommand>();

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
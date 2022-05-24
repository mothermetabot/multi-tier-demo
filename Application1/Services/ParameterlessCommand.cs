using System;
using System.Windows.Input;

namespace Application1.Services
{
    internal class ParameterlessCommand : ICommand
    {
        public ParameterlessCommand(Action command)
        {
            _command = command;
        }

        private readonly Action _command;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _command?.Invoke();
        }
    }
}

using System;
using System.Windows.Input;

namespace DirectionsInformation.Infrastructure.Commands.Base
{
    // Данный класс был взят из видеоурока Павла Шмачилина https://www.youtube.com/channel/UCsFuzbQ5KW_1f45unlvRQ1g
    internal abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}

using DirectionsInformation.Infrastructure.Commands.Base;
using System;

namespace DirectionsInformation.Infrastructure.Commands
{
    // Данный класс был взят из видеоурока Павла Шмачилина https://www.youtube.com/channel/UCsFuzbQ5KW_1f45unlvRQ1g
    internal class LambdaCommand : Command
    {
        public LambdaCommand(Action<object> Execute, Func<object, bool> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;
        // Может быть пустая ссылка, поэтому Invoke, считаем что команду можно выполнить без параметра.
        public override bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => _Execute(parameter);
    }
}

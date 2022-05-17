using System.Windows.Input;

namespace WinUIValidation
{
    public class SubmitCommand : ICommand
    {
        private readonly Action _action;

        public SubmitCommand(Action action)
        {
            _action = action;
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}

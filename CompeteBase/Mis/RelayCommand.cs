using System;
using System.Windows.Input;

namespace Compete.Mis
{
    public class RelayCommand : ICommand
    {
        public Action<object?>? ExecuteAction { get; set; } = null;

        public Func<object, bool>? CanExecuteFunc { get; set; } = null;

        //public string Text { get; set; } = string.Empty;

        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter) => CanExecuteFunc == null ? true : CanExecuteFunc(parameter!);

        public void Execute(object? parameter)
        {
            try
            {
                if (ExecuteAction != null)
                    ExecuteAction(parameter);
            }
            catch (Exception ex)
            {
                MisControls.MessageDialog.Exception(ex);
            }
        }

        protected virtual void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

using System;
using System.Windows.Input;

namespace Compete.Mis.Plugins
{
    internal abstract class PluginBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        protected virtual void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public virtual bool CanExecute(object? parameter) => parameter is PluginCommandParameter;

        public void Execute(object? parameter)
        {
            try
            {
                Run(parameter as PluginCommandParameter);
            }
            catch(Exception ex)
            {
                MisControls.MessageDialog.Exception(ex);
            }
        }

        protected abstract void Run(PluginCommandParameter? parameter);
    }
}

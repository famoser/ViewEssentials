using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Helpers;

namespace Famoser.FrameworkEssentials.View.Commands
{
    public class LoadingRelayCommand
    {
        private readonly WeakAction _execute;
        private readonly WeakFunc<bool> _canExecute;

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// </summary>
        /// <param name="execute">The execution logic.
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param><param name="canExecute">The execution status logic.</param><exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public LoadingRelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = new WeakAction(execute);
            if (canExecute != null)
                _canExecute = new WeakFunc<bool>(canExecute);
        }

        /// <summary>
        /// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand.CanExecuteChanged"/> event.
        /// 
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool _disabled;
        /// <summary>
        /// Disable the command now
        /// </summary>
        public void Disable()
        {
            _disabled = true;
            RaiseCanExecuteChanged();
        }
        
        /// <summary>
        /// Enable the command if CanExecute evaluates to true
        /// </summary>
        public void Enable()
        {
            _disabled = false;
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (_disabled)
                return false;
            if (_canExecute == null)
                return true;
            if (_canExecute.IsStatic || _canExecute.IsAlive)
                return _canExecute.Execute();
            return false;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public virtual void Execute(object parameter)
        {
            if (!CanExecute(parameter) || _execute == null || !_execute.IsStatic && !_execute.IsAlive)
                return;
            _execute.Execute();
        }
    }
}

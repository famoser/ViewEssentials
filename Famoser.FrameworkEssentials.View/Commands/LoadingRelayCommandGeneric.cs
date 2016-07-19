using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Famoser.FrameworkEssentials.View.Commands.Base;
using Famoser.FrameworkEssentials.View.Commands.Interfaces;
using GalaSoft.MvvmLight.Helpers;

namespace Famoser.FrameworkEssentials.View.Commands
{
    public class LoadingRelayCommand<T> : LoadingRelayCommandBase
    {
        private readonly WeakAction<T> _execute;
        private readonly WeakFunc<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// </summary>
        /// <param name="execute">The execution logic.
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param><param name="canExecute">The execution status logic.</param><exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public LoadingRelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = new WeakAction<T>(execute);
            if (canExecute != null)
                _canExecute = new WeakFunc<T, bool>(canExecute);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            if (!base.CanExecute(parameter))
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
        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter) || _execute == null || !_execute.IsStatic && !_execute.IsAlive)
                return;
            _execute.Execute();
        }
    }
}

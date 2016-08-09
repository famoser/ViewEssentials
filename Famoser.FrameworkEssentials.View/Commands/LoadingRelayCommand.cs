using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.View.Commands.Base;
using Famoser.FrameworkEssentials.View.Utils.Delegates;

namespace Famoser.FrameworkEssentials.View.Commands
{
    public class LoadingRelayCommand : LoadingRelayCommandBase
    {
        private readonly WeakDelegate _execute;
        private readonly WeakDelegate<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// </summary>
        /// <param name="execute">The execution logic.
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param><param name="canExecute">The execution status logic.</param>
        /// <param name="disableWhileExecuting"></param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public LoadingRelayCommand(Action execute, Func<bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakDelegate(execute);
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// </summary>
        /// <param name="execute">The execution logic.
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param><param name="canExecute">The execution status logic.</param>
        /// <param name="disableWhileExecuting"></param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public LoadingRelayCommand(Func<Task> execute, Func<bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakDelegate(execute);
        }

        private LoadingRelayCommand(Func<bool> canExecute, bool disableWhileExecuting) : base(disableWhileExecuting)
        {
            if (canExecute != null)
                _canExecute = new WeakDelegate<bool>(canExecute);
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
                return (bool)_canExecute.Execute();
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

            if (_disableWhileExecuting)
                ForceDisable();
            if (_execute.CanExecuteAsync())
            {
                _execute.ExecuteAsync().ContinueWith((e, f) =>
                {
                    if (_disableWhileExecuting)
                        ForceEnable();
                }, null);
            }
            else
            {
                _execute.Execute();
                if (_disableWhileExecuting)
                    ForceEnable();
            }
        }
    }
}

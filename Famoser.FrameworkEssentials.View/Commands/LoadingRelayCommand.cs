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
        /// Initialize the command
        /// </summary>
        /// <param name="execute">The Action to be executed</param>
        /// <param name="canExecute">A Func to determine if the command can be executed</param>
        /// <param name="disableWhileExecuting">true: CanExecute returns false while the command is excuting. Works with Async methods too if you return Task</param>
        public LoadingRelayCommand(Action execute, Func<bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakDelegate(execute);
        }

        /// <summary>
        /// Initialize the command
        /// </summary>
        /// <param name="execute">The Func to be executed</param>
        /// <param name="canExecute">A Func to determine if the command can be executed</param>
        /// <param name="disableWhileExecuting">true: CanExecute returns false while the command is excuting. Works with Async methods too if you return Task</param>
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
        /// </summary>
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

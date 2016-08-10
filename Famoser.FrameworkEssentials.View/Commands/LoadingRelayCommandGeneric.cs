using System;
using System.Reflection;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.View.Commands.Base;
using Famoser.FrameworkEssentials.View.Utils.Delegates;

namespace Famoser.FrameworkEssentials.View.Commands
{
    public class LoadingRelayCommand<T> : LoadingRelayCommandBase
    {
        private readonly WeakArgumentDelegate<T> _execute;
        private readonly WeakArgumentDelegate<T, bool> _canExecute;

        /// <summary>
        /// Initialize the command
        /// </summary>
        /// <param name="execute">The Action to be executed</param>
        /// <param name="canExecute">A Func to determine if the command can be executed</param>
        /// <param name="disableWhileExecuting">true: CanExecute returns false while the command is excuting. Works with Async methods too if you return Task</param>
        public LoadingRelayCommand(Action<T> execute, Func<T, bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakArgumentDelegate<T>(execute);
            if (canExecute != null)
                _canExecute = new WeakArgumentDelegate<T, bool>(canExecute);
        }

        /// <summary>
        /// Initialize the command
        /// </summary>
        /// <param name="execute">The Action to be executed</param>
        /// <param name="canExecute">A Func to determine if the command can be executed</param>
        /// <param name="disableWhileExecuting">true: CanExecute returns false while the command is excuting. Works with Async methods too if you return Task</param>
        public LoadingRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakArgumentDelegate<T>(execute);
            if (canExecute != null)
                _canExecute = new WeakArgumentDelegate<T, bool>(canExecute);
        }

        private LoadingRelayCommand(Func<T, bool> canExecute, bool disableWhileExecuting) : base(disableWhileExecuting)
        {
            if (canExecute != null)
                _canExecute = new WeakArgumentDelegate<T, bool>(canExecute);
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
            if (_canExecute == null)
                return true;
            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                if (parameter == null && typeof(T).GetTypeInfo().IsValueType)
                    return _canExecute.Execute(default(T));
                if (parameter == null || parameter is T)
                    return _canExecute.Execute((T)parameter);
            }
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
                if (parameter == null)
                {
                    if (typeof(T).GetTypeInfo().IsValueType)
                        _execute.ExecuteAsync(default(T)).ContinueWith((e, f) =>
                        {
                            if (_disableWhileExecuting)
                                ForceEnable();
                        }, null);
                    else
                        _execute.ExecuteAsync((T)parameter).ContinueWith((e, f) =>
                        {
                            if (_disableWhileExecuting)
                                ForceEnable();
                        }, null);
                }
                else
                    _execute.ExecuteAsync((T)parameter).ContinueWith((e, f) =>
                    {
                        if (_disableWhileExecuting)
                            ForceEnable();
                    }, null);
            }
            else
            {
                if (parameter == null)
                {
                    if (typeof(T).GetTypeInfo().IsValueType)
                        _execute.Execute(default(T));
                    else
                        _execute.Execute((T)parameter);
                }
                else
                    _execute.Execute((T)parameter);

                if (_disableWhileExecuting)
                    ForceEnable();
            }
        }
    }
}

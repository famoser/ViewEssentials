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
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// </summary>
        /// <param name="execute">The execution logic.
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param><param name="canExecute">The execution status logic.</param>
        /// <param name="disableWhileExecuting"></param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        ///             due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public LoadingRelayCommand(Action<T> execute, Func<T, bool> canExecute = null, bool disableWhileExecuting = false) : this(canExecute, disableWhileExecuting)
        {
            _execute = new WeakArgumentDelegate<T>(execute);
            if (canExecute != null)
                _canExecute = new WeakArgumentDelegate<T, bool>(canExecute);
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
        /// 
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

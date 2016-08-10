using System;
using System.Reflection;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.View.Utils.Delegates.Base;

namespace Famoser.FrameworkEssentials.View.Utils.Delegates
{
    /// <summary>
    /// Stores an <see cref="T:System.Func" /> without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakArgumentDelegate<TArgument, TResult> : BaseWeakDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakArgumentDelegate(Func<TArgument, Task<TResult>> func) : this(func?.Target, func) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakArgumentDelegate(Func<TArgument, TResult> func) : this(func?.Target, func) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="target">The func's owner.</param>
        /// <param name="func">The func that will be associated to this instance.</param>
        private WeakArgumentDelegate(object target, Delegate func)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                _staticDelegate = func;
                if (target == null)
                    return;
                Reference = new WeakReference(target);
            }
            else
            {
                Method = func.GetMethodInfo();
                FuncReference = new WeakReference(func.Target);
                Reference = new WeakReference(target);
            }
        }

        public bool CanExecuteAsync()
        {
            return _staticDelegate is Func<TArgument, Task> || Method?.ReturnType == typeof(Task); //Func<Task<T>> inherits from Func<Task>
        }

        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public async Task<TResult> ExecuteAsync(TArgument argument)
        {
            if (CanExecuteAsync())
            {
                if (_staticDelegate != null)
                {
                    if (_staticDelegate is Func<TArgument, Task<TResult>>)
                    {
                        var func = (Func<TArgument, Task<TResult>>)_staticDelegate;
                        return await func(argument);
                    }
                    if (_staticDelegate is Func<TArgument, Task>)
                    {
                        var func = (Func<TArgument, Task>)_staticDelegate;
                        await func(argument);
                    }
                }
                else
                {
                    object funcTarget = DelegateTarget;
                    if (!IsAlive || Method == null || FuncReference == null || funcTarget == null)
                        return default(TResult);
                    if (Method?.ReturnType == typeof(Task<TResult>))
                        return await (Task<TResult>)Method.Invoke(funcTarget, new object[] { argument });
                    await (Task)Method.Invoke(funcTarget, new object[] { argument });
                }
            }
            return default(TResult);
        }

        /// <summary>
        /// Executes the delegate. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public object Execute(TArgument argument)
        {
            if (_staticDelegate != null)
            {
                if (_staticDelegate is Func<TArgument, TResult>)
                {
                    var ac = (Func<TArgument, TResult>)_staticDelegate;
                    return ac.Invoke(argument);
                }
                if (_staticDelegate is Func<TArgument, Task<TResult>>)
                {
                    var ac = (Func<TArgument, Task<TResult>>)_staticDelegate;
                    return ac.Invoke(argument);
                }
            }
            else
            {
                object funcTarget = DelegateTarget;
                if (!IsAlive || Method == null || FuncReference == null || funcTarget == null)
                    return default(TResult);
                
                return Method.Invoke(funcTarget, new object[] { argument });
            }
            return default(TResult);
        }
    }
}

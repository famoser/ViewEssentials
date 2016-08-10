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
    public class WeakDelegate : BaseWeakDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakDelegate(Action func) : this(func?.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakDelegate(Func<Task> func) : this(func?.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="target">The func's owner.</param>
        /// <param name="func">The func that will be associated to this instance.</param>
        private WeakDelegate(object target, Delegate func)
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
            return _staticDelegate is Func<Task> || Method?.ReturnType == typeof(Task); //Func<Task<T>> inherits from Func<Task>
        }

        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (CanExecuteAsync())
            {
                if (_staticDelegate != null)
                {
                    if (_staticDelegate is Func<Task>)
                    {
                        var func = (Func<Task>) _staticDelegate;
                        await func();
                    }
                }
                else
                {
                    object funcTarget = DelegateTarget;
                    if (!IsAlive || Method == null || FuncReference == null || funcTarget == null)
                        return;
                    await (Task)Method.Invoke(funcTarget, null);
                }
            }
        }

        /// <summary>
        /// Executes the delegate. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (_staticDelegate != null)
            {
                if (_staticDelegate is Action)
                {
                    var ac = (Action)_staticDelegate;
                    ac.Invoke();
                }
                if (_staticDelegate is Func<Task>)
                {
                    var ac = (Func<Task>)_staticDelegate;
                    ac.Invoke();
                }
            }
            else
            {
                object funcTarget = DelegateTarget;
                if (!IsAlive || Method == null || FuncReference == null || funcTarget == null)
                    return;
                Method.Invoke(funcTarget, null);
            }
        }
    }
}

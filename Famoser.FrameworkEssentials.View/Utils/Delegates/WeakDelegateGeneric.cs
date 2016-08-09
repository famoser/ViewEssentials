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
    public class WeakDelegate<TResult> : BaseWeakDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakDelegate(Func<Task<TResult>> func) : this(func?.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc" /> class.
        /// </summary>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakDelegate(Func<TResult> func) : this(func?.Target, func)
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
            return _staticDelegate is Func<Task<TResult>>; //Func<Task<T>> inherits from Func<Task>
        }

        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public async Task<TResult> ExecuteAsync()
        {
            if (_staticDelegate is Func<Task<TResult>>)
            {
                var func = (Func<Task<TResult>>)_staticDelegate;
                return await func();
            }
            return default(TResult);
        }

        /// <summary>
        /// Executes the delegate. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public object Execute()
        {
            if (_staticDelegate != null)
            {
                if (_staticDelegate is Func<TResult>)
                {
                    var ac = (Func<TResult>)_staticDelegate;
                    return ac.Invoke();
                }
            }
            else
            {
                object funcTarget = DelegateTarget;
                if (!IsAlive || Method == null || FuncReference == null || funcTarget == null)
                    return default(TResult);
                return Method.Invoke(funcTarget, null);
            }
            return default(TResult);
        }
    }
}

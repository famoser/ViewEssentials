using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.FrameworkEssentials.View.Utils
{
    /// <summary>
    /// Stores an <see cref="T:System.Func" /> without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakDelegate
    {
        private Delegate _staticDelegate;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (_staticDelegate != null)
                    return _staticDelegate.GetMethodInfo().Name;
                return Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's func's target.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference FuncReference { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc.FuncReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Gets a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic => _staticDelegate != null;

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticDelegate == null && Reference == null)
                    return false;
                if (_staticDelegate != null && Reference == null)
                    return true;
                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target => Reference?.Target;

        /// <summary>The target of the weak reference.</summary>
        protected object DelegateTarget => FuncReference?.Target;

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
            return _staticDelegate is Func<Task>; //Func<Task<T>> inherits from Func<Task>
        }

        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (_staticDelegate is Func<Task>)
            {
                var func = (Func<Task>)_staticDelegate;
                await func();
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

        /// <summary>Sets the reference that this instance stores to null.</summary>
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticDelegate = null;
        }
    }
}

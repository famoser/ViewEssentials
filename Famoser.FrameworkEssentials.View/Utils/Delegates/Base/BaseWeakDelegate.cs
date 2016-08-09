using System;
using System.Reflection;

namespace Famoser.FrameworkEssentials.View.Utils.Delegates.Base
{
    public class BaseWeakDelegate
    {
        protected Delegate _staticDelegate;

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

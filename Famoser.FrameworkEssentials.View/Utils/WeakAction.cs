using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.FrameworkEssentials.View.Utils
{
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action _staticAction;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (_staticAction != null)
                    return _staticAction.GetMethodInfo().Name;
                return Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference ActionReference { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public bool IsStatic => _staticAction != null;

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null)
                    return false;
                if (_staticAction != null && Reference == null)
                    return true;
                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target => Reference?.Target;

        /// <summary>The target of the weak reference.</summary>
        protected object ActionTarget => ActionReference?.Target;

        /// <summary>
        /// Initializes an empty instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action action) : this(action?.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;
                if (target == null)
                    return;
                Reference = new WeakReference(target);
            }
            else
            {
                Method = action.GetMethodInfo();
                ActionReference = new WeakReference(action.Target);
                Reference = new WeakReference(target);
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
            }
            else
            {
                object actionTarget = ActionTarget;
                if (!IsAlive || Method == null || (ActionReference == null || actionTarget == null))
                    return;
                Method.Invoke(actionTarget, null);
            }
        }

        /// <summary>Sets the reference that this instance stores to null.</summary>
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            Method = null;
            _staticAction = null;
        }
    }
}

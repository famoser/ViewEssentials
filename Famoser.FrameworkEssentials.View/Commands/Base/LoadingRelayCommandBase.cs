using System;
using System.Collections.Generic;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands.Disposables;
using Famoser.FrameworkEssentials.View.Commands.Interfaces;

namespace Famoser.FrameworkEssentials.View.Commands.Base
{
    public abstract class LoadingRelayCommandBase : ILoadingRelayCommand
    {
        protected readonly bool _disableWhileExecuting;

        protected LoadingRelayCommandBase(bool disableWhileExecuting)
        {
            _disableWhileExecuting = disableWhileExecuting;
        }

        private List<ILoadingRelayCommand> _dependentCommands = new List<ILoadingRelayCommand>();
        public virtual bool CanExecute(object parameter)
        {
            return !_disabled && !_forceDisable;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand.CanExecuteChanged"/> event.
        /// 
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool _disabled;
        /// <summary>
        /// Disable the command now
        /// </summary>
        public void Disable()
        {
            _disabled = true;
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Enable the command if CanExecute evaluates to true
        /// </summary>
        public void Enable()
        {
            _disabled = false;
            RaiseCanExecuteChanged();
        }

        private bool _forceDisable;
        /// <summary>
        /// Disable the command now
        /// </summary>
        protected void ForceDisable()
        {
            _forceDisable = true;
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Enable the command if CanExecute evaluates to true
        /// </summary>
        protected void ForceEnable()
        {
            _forceDisable = false;
            RaiseCanExecuteChanged();
        }

        public void AddDependentCommand(ILoadingRelayCommand command)
        {
            _dependentCommands.Add(command);
        }

        public ShowIndeterminateProgressDisposable GetProgressDisposable(IProgressService progressService, object progressKey, bool disableCommand = true)
        {
            return new ShowIndeterminateProgressDisposable(this, _dependentCommands, progressService, progressKey, disableCommand);
        }

        public ShowIndeterminateProgressDisposable GetProgressDisposable()
        {
            return new ShowIndeterminateProgressDisposable(this, _dependentCommands);
        }
    }
}

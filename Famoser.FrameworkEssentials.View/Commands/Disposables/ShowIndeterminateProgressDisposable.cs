using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands.Interfaces;

namespace Famoser.FrameworkEssentials.View.Commands.Disposables
{
    public class ShowIndeterminateProgressDisposable : IDisposable
    {
        private readonly ILoadingRelayCommand _command;
        private readonly IList<ILoadingRelayCommand> _dependentCommands;
        private readonly IProgressService _progressService;
        private readonly object _progressKey;
        private readonly bool _disableCommand;

        public ShowIndeterminateProgressDisposable(ILoadingRelayCommand command, IList<ILoadingRelayCommand> dependentCommands, IProgressService progressService = null, object progressKey = null, bool disableCommand = true)
        {
            _command = command;
            _dependentCommands = dependentCommands;
            _progressService = progressService;
            _progressKey = progressKey;
            _disableCommand = disableCommand;

            Start();
        }

        private void Start()
        {
            _progressService?.StartIndeterminateProgress(_progressKey);
            if (_disableCommand)
            {
                foreach (var loadingRelayCommand in _dependentCommands)
                {
                    loadingRelayCommand.Disable();
                    loadingRelayCommand.RaiseCanExecuteChanged();
                }

                _command.Disable();
                _command.RaiseCanExecuteChanged();
            }
        }

        public void Dispose()
        {
            _progressService?.StopIndeterminateProgress(_progressKey);
            if (_disableCommand)
            {
                foreach (var loadingRelayCommand in _dependentCommands)
                {
                    loadingRelayCommand.Enable();
                    loadingRelayCommand.RaiseCanExecuteChanged();
                }

                _command.Enable();
                _command.RaiseCanExecuteChanged();
            }
        }
    }
}

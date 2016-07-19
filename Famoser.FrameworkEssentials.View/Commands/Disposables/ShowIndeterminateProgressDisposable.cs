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
        private readonly IProgressService _progressService;
        private readonly object _progressKey;
        private readonly bool _disableCommand;

        public ShowIndeterminateProgressDisposable(ILoadingRelayCommand command, IProgressService progressService, object progressKey, bool disableCommand = true)
        {
            _command = command;
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
                _command.Disable();
                _command.RaiseCanExecuteChanged();
            }
        }

        public void Dispose()
        {
            _progressService.StopIndeterminateProgress(_progressKey);
            if (_disableCommand)
            {
                _command.Enable();
                _command.RaiseCanExecuteChanged();
            }
        }
    }
}

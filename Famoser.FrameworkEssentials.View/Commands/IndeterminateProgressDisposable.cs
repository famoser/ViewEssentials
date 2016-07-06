using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Famoser.FrameworkEssentials.View.Commands
{
    public class IndeterminateProgressDisposable<T, T2> : IDisposable
    {
        private readonly RelayCommand _relayCommand;
        private readonly RelayCommand<T2> _genericRelayCommand;

        private readonly Action<bool> _booleanSetter;
        private readonly T _progressKey;
        private readonly IProgressService _progressService;

        public IndeterminateProgressDisposable(RelayCommand command, Action<bool> booleanSetter, T key, IProgressService progressService)
        {
            _relayCommand = command;
            _booleanSetter = booleanSetter;
            _progressKey = key;
            _progressService = progressService;

            Start();
        }

        public IndeterminateProgressDisposable(RelayCommand<T2> command, Action<bool> booleanSetter, T key, IProgressService progressService)
        {
            _genericRelayCommand = command;
            _booleanSetter = booleanSetter; 
            _progressKey = key;
            _progressService = progressService;

            Start();
        }

        private void Start()
        {
            _progressService.StartIndeterminateProgress(_progressKey);
            _booleanSetter(true);
            _relayCommand?.RaiseCanExecuteChanged();
            _genericRelayCommand?.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            _progressService.StopIndeterminateProgress(_progressKey);
            _booleanSetter(false);
            _relayCommand?.RaiseCanExecuteChanged();
            _genericRelayCommand?.RaiseCanExecuteChanged();
        }
    }
}

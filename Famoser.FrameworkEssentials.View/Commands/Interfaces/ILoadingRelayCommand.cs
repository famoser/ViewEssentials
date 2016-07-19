using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Famoser.FrameworkEssentials.View.Commands.Interfaces
{
    public interface ILoadingRelayCommand : ICommand
    {
        /// <summary>
        /// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand.CanExecuteChanged"/> event.
        /// 
        /// </summary>
        void RaiseCanExecuteChanged();
        
        /// <summary>
        /// Disable the command now
        /// </summary>
        void Disable();

        /// <summary>
        /// Enable the command if CanExecute evaluates to true
        /// </summary>
        void Enable();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.Services
{
    class NotifyBoxContext
    {
        public string Message { get; set; }
        public NotifyBoxContext(string Message)
        {
            this.Message = Message;
        }
        public string CloseIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/CloseIcon.png"; }
        }
        public string AppIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/AppIco.ico"; }
        }
        public string CheckIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/check.png"; }
        }

        #region CloseWindowCommand

        private Command.RelayCommand closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get
            {
                if (closeWindowCommand == null)
                {
                    closeWindowCommand = new Command.RelayCommand(CloseWindow,CanCloseWindow);
                }
                return closeWindowCommand;
            }
        }

        private void CloseWindow(object obj)
        {
            ServiceManager.CallService("CloseWindow", obj);
        }

        private bool CanCloseWindow(object obj)
        {
            return obj is Window;
        }
        #endregion

        #region DragMoveCommand

        private Command.RelayCommand dragMoveCommand;
        public ICommand DragMoveCommand
        {
            get
            {
                if (dragMoveCommand == null)
                {
                    dragMoveCommand = new Command.RelayCommand(DragMove, CanDragMove);
                }
                return dragMoveCommand;
            }
        }

        private void DragMove(object obj)
        {
            (obj as Window).DragMove();
        }

        private bool CanDragMove(object obj)
        {
            return obj is Window;
        }
        #endregion
    }
}

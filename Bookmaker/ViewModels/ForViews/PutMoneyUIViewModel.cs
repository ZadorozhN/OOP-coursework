using Bookmaker.Interfaces;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class PutMoneyUIViewModel : ViewModelBase, IPageViewModel, IDataErrorInfo
    {
        private UserViewModel activeUser;
        public UserViewModel ActiveUser
        {
            get
            {
                return activeUser;
            }
            set
            {
                activeUser = value;
                OnPropertyChanged("ActiveUser");
            }
        }
        public UnitOfWork BC { get; set; }
        private decimal cash;
        public decimal Cash
        {
            get
            {
                return cash;
            }
            set
            {
                cash = value;
                OnPropertyChanged("Cash");
            }
        }

        public PutMoneyUIViewModel(UnitOfWork bc)
        {
            BC = bc;
        }

        #region PutMoneyCommand

        private Command.RelayCommand putMoneyCommand;
        public ICommand PutMoneyCommand
        {
            get
            {
                if (putMoneyCommand == null)
                {
                    putMoneyCommand = new Command.RelayCommand(PutMoney,CanPutMoney);
                }
                return putMoneyCommand;
            }
        }
        private void PutMoney(object obj)
        { 
            BC.Refresh();
            if (AccountCheck())
            {
                ActiveUser.PutMoney(Cash);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        private bool CanPutMoney(object obj)
        {
            if (this.IsValid())
                return true;
            return false;
        }
        #endregion

        #region OutputMoneyCommand

        private Command.RelayCommand outputMoneyCommand;
        public ICommand OutputMoneyCommand
        {
            get
            {
                if (outputMoneyCommand == null)
                {
                    outputMoneyCommand = new Command.RelayCommand(OutputMoney, CanOutputMoney);
                }
                return outputMoneyCommand;
            }
        }
        private void OutputMoney(object obj)
        {

            BC.Refresh();
            if (AccountCheck()) 
            {
                ActiveUser.OutputMoney(Cash);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        private bool CanOutputMoney(object obj)
        {
            
            if (ActiveUser.IsValid() && this.IsValid() && Cash <= ActiveUser.Cash)
                return true;
            return false;
        }
        #endregion

        #region BackToPersonalAccountCommand

        private Command.RelayCommand backToPersonalAccountCommand;
        public ICommand BackToPersonalAccountCommand
        {
            get
            {
                if (backToPersonalAccountCommand == null)
                {
                    backToPersonalAccountCommand = new Command.RelayCommand(BackToPersonalAccount);
                }
                return backToPersonalAccountCommand;
            }
        }
        private void BackToPersonalAccount(object obj)
        {
            BC.Refresh();
            if (AccountCheck())
            {
                Mediator.Mediator.Notify("GoToPersonalAccountUIScreen", ActiveUser);
                this.ActiveUser = null;
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
                ServiceManager.CallService("CloseApp", null);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "Cash":
                        result = this.CashValidation();
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        #endregion

        #region ValidationMethods

        public void Reset()
        {
            this.Cash = default(decimal);
        }

        public bool IsValid()
        {
            var CashValid = this.CashValidation();

            var result = CashValid == null;

            return result;
        }
        private string CashValidation()
        {
            string result = null;

            if (this.Cash < 0)
                result = "Incorrect Cash";

            return result;
        }
        #endregion
        private bool AccountCheck()
        {
            IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                              where u.Id == ActiveUser.Id
                                              select u;
            if (query.Count() != 0 && !query.First().IsBlocked)
            {
                return true;
            }
            return false;
        }
    }
}

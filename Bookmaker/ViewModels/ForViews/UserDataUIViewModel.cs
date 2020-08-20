using Bookmaker.Interfaces;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class UserDataUIViewModel : ViewModelBase, IPageViewModel
    {
        public UserViewModel activeUser;
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
        public UserDataUIViewModel(UnitOfWork bc)
        {
            BC = bc;
        }

        #region SaveUserChangesCommand

        private Command.RelayCommand saveUserChangesCommand;
        public ICommand SaveUserChangesCommand
        {
            get
            {
                if (saveUserChangesCommand == null)
                {
                    saveUserChangesCommand = new Command.RelayCommand(SaveUserChanges, CanSaveUserChanges);
                }
                return saveUserChangesCommand;
            }
        }

        private void SaveUserChanges(object obj)
        {
            if (AccountCheck())
            {
                BC.Users.Update(ActiveUser);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        private bool CanSaveUserChanges(object obj)
        {
            return true && ActiveUser.IsValid();
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
                    backToPersonalAccountCommand = new Command.RelayCommand(BackToPersonalAccount, CanBackToPersonalAccount);
                }
                return backToPersonalAccountCommand;
            }
        }
        private void BackToPersonalAccount(object obj)
        {
            BC.Refresh();
            if (AccountCheck())
            {
                if (ActiveUser.IsAdministrator)
                {
                    Mediator.Mediator.Notify("GoToAdministratorUIScreen", ActiveUser);
                    this.ActiveUser = null;
                }
                else
                {
                    Mediator.Mediator.Notify("GoToPersonalAccountUIScreen", ActiveUser);
                    this.ActiveUser = null;
                }
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
                ServiceManager.CallService("CloseApp", null);
            }
        }

        private bool CanBackToPersonalAccount(object obj)
        {
            return ActiveUser.IsValid();
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

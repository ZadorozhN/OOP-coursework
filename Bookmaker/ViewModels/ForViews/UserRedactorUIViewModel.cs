using Bookmaker.Interfaces;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class UserRedactorUIViewModel : ViewModelBase, IPageViewModel
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
        public UserViewModel administrator;
        public UserViewModel Administrator
        {
            get
            {
                return administrator;
            }
            set
            {
                administrator = value;
                OnPropertyChanged("Administrator");
            }
        }
        public UnitOfWork BC { get; set; }
        public UserRedactorUIViewModel(UnitOfWork bc)
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
            IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                              where u.Id == ActiveUser.Id
                                              select u;

            if (query.Count() != 0)
            {
                BC.Users.Update(ActiveUser);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.UserDeleted);
                BackToAdministratorAccount(null);
            }
        }

        private bool CanSaveUserChanges(object obj)
        {
            return true && ActiveUser.IsValid();
        }
        #endregion

        #region DeleteUserCommand

        private Command.RelayCommand deleteUserCommand;
        public ICommand DeleteUserCommand
        {
            get
            {
                if (deleteUserCommand == null)
                {
                    deleteUserCommand = new Command.RelayCommand(DeleteUser,CanDeleteUser);
                }
                return deleteUserCommand;
            }
        }

        private void DeleteUser(object obj)
        {
            BC.FullRefresh();

            IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                              where u.Id == ActiveUser.Id
                                              select u;

            if (query.Count() != 0)
            {
                if (!query.First().IsAdministrator)
                {

                    ((DbSet<BetViewModel>)(BC.Bets.GetAll())).RemoveRange(ActiveUser.Bets);
                    PassportViewModel passport = ActiveUser.Passport;
                    BC.Users.Delete(ActiveUser.Id);
                    BC.Passports.Delete(passport.Id);
                    ActiveUser = null;
                    BC.Save();
                    BC.FullRefresh();
                    BackToAdministratorAccount(null);
                }
                else
                {
                    ServiceManager.CallService("ShowNotifyBox", Properties.Resources.UserCannotBeDeleted);
                }

            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.UserDeleted);
                BackToAdministratorAccount(null);
            }
        }

        private bool CanDeleteUser(object obj)
        {
            if(ActiveUser != Administrator)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region BackToAdministratorAccountCommand

        private Command.RelayCommand backToAdministratorAccountCommand;
        public ICommand BackToAdministratorAccountCommand
        {
            get
            {
                if (backToAdministratorAccountCommand == null)
                {
                    backToAdministratorAccountCommand = new Command.RelayCommand(BackToAdministratorAccount, CanBackToAdministratorAccount);
                }
                return backToAdministratorAccountCommand;
            }
        }
        private void BackToAdministratorAccount(object obj)
        {
            BC.FullRefresh();
            Mediator.Mediator.Notify("GoToAdministratorUIScreen", Administrator);
            this.ActiveUser = null;
        }

        private bool CanBackToAdministratorAccount(object obj)
        {
            return ActiveUser.IsValid();
        }

        #endregion
    }
}

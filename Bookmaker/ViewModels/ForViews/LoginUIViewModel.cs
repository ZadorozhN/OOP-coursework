using Bookmaker.Interfaces;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bookmaker.ViewModels
{
    class LoginUIViewModel : ViewModelBase, IPageViewModel
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

        #region Constructor        
        public LoginUIViewModel(UnitOfWork bc)
        {
            BC = bc;
            ActiveUser = new UserViewModel();
        }
        #endregion

        #region AuthorizationUserCommand

        private Command.RelayCommand authorizationUserCommand;
        public ICommand AuthorizationUserCommand
        {
            get
            {
                if (authorizationUserCommand == null)
                {
                    authorizationUserCommand = new Command.RelayCommand(AuthorizationUser,CanAuthorizationUser);
                }
                return authorizationUserCommand;
            }
        }
        private void AuthorizationUser(object obj)
        {
            BC.Refresh();
            PasswordBox passwordBox = obj as PasswordBox;
            var query = from u in (DbSet<UserViewModel>)(BC.Users.GetAll())
                        join p in (DbSet<PassportViewModel>)(BC.Passports.GetAll()) on u.Passport equals p
                        where u.UserName == ActiveUser.UserName
                        select new { user = u, passport = p };
            BC.Refresh();
            if (query.Count() != 0)
            {
                ActiveUser = query.First().user;
                ActiveUser.Passport = query.First().passport;

                if (ActiveUser != null && ActiveUser.Passport != null && SaltedHash.Verify(ActiveUser.Salt, ActiveUser.Hash, passwordBox.Password))
                {
                    if (!ActiveUser.IsBlocked)
                    {
                        if (ActiveUser.IsAdministrator)
                        {
                            BC.FullRefresh();
                            Mediator.Mediator.Notify("GoToAdministratorUIScreen", ActiveUser);
                        }
                        else
                        {
                            BC.Refresh();
                            Mediator.Mediator.Notify("GoToPersonalAccountUIScreen", ActiveUser);
                        }
                    }
                    else
                    {
                        ServiceManager.CallService("ShowNotifyBox", Resources.UserBlocked);
                    }

                }
                else
                {
                    ServiceManager.CallService("ShowNotifyBox", Resources.IncorrectPasswordOrLogin);
                }

            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.IncorrectPasswordOrLogin);
            }
            ActiveUser = new UserViewModel();
        }
        private bool CanAuthorizationUser(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            if (passwordBox != null && ActiveUser.UserName != null) {

                bool valid = true;
                Regex rule1 = new Regex(@"^[0-9a-zA-Z*]{6,32}$");

                if (string.IsNullOrEmpty(passwordBox.Password))
                    valid = false;
                else if (!rule1.IsMatch(passwordBox.Password))
                    valid = false;

                return valid; 
            }
            return false;
        }

        #endregion

        #region GoToRegisterUIViewCommand

        private Command.RelayCommand goToRegisterUIViewCommand;
        public ICommand GoToRegisterUIViewCommand
        {
            get
            {
                if (goToRegisterUIViewCommand == null)
                {
                    goToRegisterUIViewCommand = new Command.RelayCommand(GoToRegisterUIView);
                }
                return goToRegisterUIViewCommand;
            }
        }
        private void GoToRegisterUIView(object obj)
        {
            Mediator.Mediator.Notify("GoToRegisterUIScreen", "");
        }

        #endregion

        #region GoToVerifyingUIViewCommand

        private Command.RelayCommand goToVerifyingUIViewCommand;
        public ICommand GoToVerifyingUIViewCommand
        {
            get
            {
                if (goToVerifyingUIViewCommand == null)
                {
                    goToVerifyingUIViewCommand = new Command.RelayCommand(GoToVerifyingUIView);
                }
                return goToVerifyingUIViewCommand;
            }
        }

        public string Password { get; private set; }

        private void GoToVerifyingUIView(object obj)
        {
            Mediator.Mediator.Notify("GoToVerifyingUIScreen", "");
        }

        #endregion        
    }
}

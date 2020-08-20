using Bookmaker.Interfaces;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class RegisterUIViewModel : ViewModelBase, IPageViewModel
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
        public RegisterUIViewModel(UnitOfWork bc)
        {
            BC = bc;
            ActiveUser = new UserViewModel();
        }
        #endregion

        #region RegisterUserCommand

        private Command.RelayCommand registerUserCommand;
        public ICommand RegisterUserCommand
        {
            get
            {
                if (registerUserCommand == null)
                {
                    registerUserCommand = new Command.RelayCommand(RegisterUser,CanRegisterUser);
                }
                return registerUserCommand;
            }
        }
        private void RegisterUser(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            Regex rule1 = new Regex(@"^[0-9a-zA-Z*]{6,32}$");

            if (rule1.IsMatch(passwordBox.Password))
            {

            
            SaltedHash saltedHash = new SaltedHash(passwordBox.Password);
            ActiveUser.Hash = saltedHash.Hash;
            ActiveUser.Salt = saltedHash.Salt;
                if (ActiveUser.IsValid())
                {

                    IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                                      where u.UserName == ActiveUser.UserName
                                                      select u;
                    IQueryable<PassportViewModel> queryPassport = from p in ((DbSet<PassportViewModel>)(BC.Passports.GetAll()))
                                                                  where p.Id == ActiveUser.Passport.Id
                                                                  select p;
                    IQueryable<UserViewModel> queryEmail = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                                           where u.Email == ActiveUser.Email
                                                           select u;

                    if (queryEmail.Count() == 0)
                    {
                        if (query.Count() == 0)
                        {
                            if (queryPassport.Count() == 0)
                            {
                                ActiveUser.RegisterDate = DateTime.Now;
                                BC.Users.Create(ActiveUser);
                                BC.Save();
                                GoToLoginUIView(null);
                                ActiveUser = new UserViewModel();
                            }
                            else
                            {
                                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.PassportExists);
                            }
                        }
                        else
                        {
                            ServiceManager.CallService("ShowNotifyBox", Properties.Resources.UserNameExists);
                        }
                    }
                    else
                    {
                        ServiceManager.CallService("ShowNotifyBox", Properties.Resources.EmailExists);
                    }
                }
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.IncorrectPassword);
            }
        }
        private bool CanRegisterUser(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            if (passwordBox != null && !string.IsNullOrEmpty(passwordBox.Password) && ActiveUser.IsValid())
            {
                return true;
            }
            return false;
        }

        #endregion

        #region GoToLoginUIViewCommand

        private Command.RelayCommand goToLoginUIViewCommand;
        public ICommand GoToLoginUIViewCommand
        {
            get
            {
                if (goToLoginUIViewCommand == null)
                {
                    goToLoginUIViewCommand = new Command.RelayCommand(GoToLoginUIView);
                }
                return goToLoginUIViewCommand;
            }
        }
        private void GoToLoginUIView(object obj)
        {
            Mediator.Mediator.Notify("GoToLoginUIScreen", "");
        }

        #endregion
    }
}

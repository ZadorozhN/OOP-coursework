using Bookmaker.Interfaces;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class VerifyingUIViewModel : ViewModelBase, IPageViewModel
    {
        public UnitOfWork BC { get; set; }
        private string recoveryCode { get; set; }
        private bool canChangePassword { get; set; }
        private UserViewModel recoveryUser { get; set; }
        public VerifyingUIViewModel(UnitOfWork bc)
        {
            BC = bc;
        }

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

        #region RestorePasswordCommand

        private Command.RelayCommand restorePasswordCommand;
        public ICommand RestorePasswordCommand
        {
            get
            {
                if (restorePasswordCommand == null)
                {
                    restorePasswordCommand = new Command.RelayCommand(RestorePassword);
                }
                return restorePasswordCommand;
            }
        }
        private void RestorePassword(object obj)
        {
            string email = obj as string;
            if (email != null)
            {
                BC.FullRefresh();
                IQueryable<UserViewModel> query = from u in (DbSet<UserViewModel>)(BC.Users.GetAll())
                                                  where u.Email == email
                                                  select u;
                if (query.Count() != 0)
                {
                    recoveryUser = query.First();

                    byte[] recoveryBytes = new byte[8];
                    new Random().NextBytes(recoveryBytes);
                    recoveryCode = Convert.ToBase64String(recoveryBytes);

                    ServiceManager.CallService("SendEmail", (recoveryUser.Email, "Код восстановления " + recoveryCode, "Восстановление пароля"));
                    ServiceManager.CallService("ShowNotifyBox", Properties.Resources.Ok);
                }
                else
                {
                    ServiceManager.CallService("ShowNotifyBox", Resources.UserIsNotFound);
                }
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.EnterYourEmail);
            }
        }
        #endregion

        #region RestorePasswordCommand

        private Command.RelayCommand checkRecoveryCodeCommand;
        public ICommand CheckRecoveryCodeCommand
        {
            get
            {
                if (checkRecoveryCodeCommand == null)
                {
                    checkRecoveryCodeCommand = new Command.RelayCommand(CheckRecoveryCode,CanCheckRecoveryCode);
                }
                return checkRecoveryCodeCommand;
            }
        }
        private void CheckRecoveryCode(object obj)
        {
            string code = obj as string;
            if (recoveryCode == code)
            {
                canChangePassword = true;
                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.EnterNewPassword);
            }
            else
            {
                canChangePassword = false;
            }
        }

        private bool CanCheckRecoveryCode(object obj)
        {
            bool valid = true;
            if (string.IsNullOrEmpty(obj as string))
                valid = false;
            return valid;
        }
        #endregion

        #region ChangePasswordCommand

        private Command.RelayCommand changePasswordCommand;
        public ICommand ChangePasswordCommand
        {
            get
            {
                if (changePasswordCommand == null)
                {
                    changePasswordCommand = new Command.RelayCommand(ChangePassword,CanChangePassword);
                }
                return changePasswordCommand;
            }
        }
        private void ChangePassword(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;

            SaltedHash saltedHash = new SaltedHash(passwordBox.Password);

            recoveryUser.Hash = saltedHash.Hash;
            recoveryUser.Salt = saltedHash.Salt;

            BC.Users.Update(recoveryUser);
            BC.Save();
            ServiceManager.CallService("ShowNotifyBox", Properties.Resources.PasswordChanged);
            GoToLoginUIView(null);
            recoveryCode = null;
            canChangePassword = false;
            recoveryUser = null;
        }

        private bool CanChangePassword(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            if (passwordBox != null && recoveryUser != null)
            {
                bool valid = true;
                Regex rule1 = new Regex(@"^[0-9a-zA-Z*]{6,32}$");

                if (string.IsNullOrEmpty(passwordBox.Password))
                    valid = false;
                else if (!rule1.IsMatch(passwordBox.Password))
                    valid = false;
                return valid && canChangePassword;
            }
            return false;

        }
        #endregion
    }
}


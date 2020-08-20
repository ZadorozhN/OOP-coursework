using Bookmaker.Config;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class SettingsUIViewModel : ViewModelBase, IPageViewModel
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
        public SettingsUIViewModel(UnitOfWork bc)
        {
            BC = bc;
        }        
        public string ActiveLanguageIcon
        {
            get
            {
                if(ConfigManager.GetConfigManager.GetActiveLanguage() == "en-US")
                {
                    return ENGFlagIcon;
                }
                else 
                {
                    return RUSFlagIcon;
                }
            }
        }

        #region Images

        public string RUSFlagIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/RUS.jpg"; }
        }
        public string ENGFlagIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/ENG.jpg"; }
        }
        public string Background1
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background1.jpg"; }
        }
        public string Background2
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background2.jpg"; }
        }
        public string Background3
        {
            get {return @"/Bookmaker;component/Resources/Backgrounds/background3.jpg"; }
        }
        public string Background4
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background4.jpg"; }
        }
        public string Background5
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background5.jpg"; }
        }
        public string Background6
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background6.jpg"; }
        }
        public string Background7
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background7.jpg"; }
        }
        public string Background8
        {
            get { return @"/Bookmaker;component/Resources/Backgrounds/background8.jpg"; }
        }
        #endregion

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

        #region ChangePasswordCommand

        private Command.RelayCommand changePasswordCommand;
        public ICommand ChangePasswordCommand
        {
            get
            {
                if (changePasswordCommand == null)
                {
                    changePasswordCommand = new Command.RelayCommand(ChangePassword, CanChangePassword);
                }
                return changePasswordCommand;
            }
        }
        private void ChangePassword(object obj)
        {
            if (AccountCheck())
            {

                PasswordBox passwordBox = obj as PasswordBox;

                SaltedHash saltedHash = new SaltedHash(passwordBox.Password);

                ActiveUser.Hash = saltedHash.Hash;
                ActiveUser.Salt = saltedHash.Salt;

                BC.Users.Update(ActiveUser);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        private bool CanChangePassword(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            if (passwordBox != null)
            {
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

        #endregion

        #region SwapLanguageCommand

        private Command.RelayCommand swapLanguageCommand;
        public ICommand SwapLanguageCommand
        {
            get
            {
                if (swapLanguageCommand == null)
                {
                    swapLanguageCommand = new Command.RelayCommand(SwapLanguage);
                }
                return swapLanguageCommand;
            }
        }
        private void SwapLanguage(object obj)
        {
            if(ConfigManager.GetConfigManager.GetActiveLanguage() == "en-US")
            {
                ConfigManager.GetConfigManager.WriteLanguageConfig("ru-Ru");
                ConfigManager.GetConfigManager.SetAppLanguage();
                OnPropertyChanged("ActiveLanguageIcon");
            }
            else
            {
                ConfigManager.GetConfigManager.WriteLanguageConfig("en-US");
                ConfigManager.GetConfigManager.SetAppLanguage();
                OnPropertyChanged("ActiveLanguageIcon");
            }
            BackToPersonalAccount(null);
        }

        #endregion

        #region SwapColorCommand

        private Command.RelayCommand swapColorCommand;
        public ICommand SwapColorCommand
        {
            get
            {
                if (swapColorCommand == null)
                {
                    swapColorCommand = new Command.RelayCommand(SwapColor,CanSwapColor);
                }
                return swapColorCommand;
            }
        }
        private void SwapColor(object obj)
        {
            ConfigManager.GetConfigManager.WriteColorConfig(obj as string);
            ConfigManager.GetConfigManager.SetActiveColor();
        }

        private bool CanSwapColor(object obj)
        {
            
            if(obj is string)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region SwapThemeCommand

        private Command.RelayCommand swapThemeCommand;
        public ICommand SwapThemeCommand
        {
            get
            {
                if (swapThemeCommand == null)
                {
                    swapThemeCommand = new Command.RelayCommand(SwapTheme);
                }
                return swapThemeCommand;
            }
        }
        private void SwapTheme(object obj)
        {
            ConfigManager.GetConfigManager.SwapTheme();
            ConfigManager.GetConfigManager.SetAppTheme();
        }
        #endregion

        #region SwapBackgroundCommand

        private Command.RelayCommand swapBackgroundCommand;
        public ICommand SwapBackgroundCommand
        {
            get
            {
                if (swapBackgroundCommand == null)
                {
                    swapBackgroundCommand = new Command.RelayCommand(SwapBackground, CanSwapBackground);
                }
                return swapBackgroundCommand;
            }
        }
        private void SwapBackground(object obj)
        {
            ConfigManager.GetConfigManager.WriteBackgroundConfig(obj as string);
            ConfigManager.GetConfigManager.SetActiveColor();
        }

        private bool CanSwapBackground(object obj)
        {

            if (obj is string)
            {
                return true;
            }
            return false;
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

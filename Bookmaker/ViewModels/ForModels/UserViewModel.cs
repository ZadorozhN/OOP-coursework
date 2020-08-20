using Bookmaker.Models;
using Bookmaker.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    [Table("Users")]
    class UserViewModel : ViewModelBase, IDataErrorInfo
    {
        private User User;
        private PassportViewModel passport;
        private BindingList<BetViewModel> bets;
        public UserViewModel()
        {
            this.User = new User(0, null, false, null, 0, null, null);
            this.passport = new PassportViewModel();
            this.bets = new BindingList<BetViewModel>();
            this.bets.ListChanged += StatisticsChange;
        }

        private void StatisticsChange(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("TotalWins");
            OnPropertyChanged("TotalDefeats");
            OnPropertyChanged("TotalUndefined");
        }

        public int Id
        {
            get { return User.Id; }
            set
            {
                User.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string UserName
        {
            get { return User.UserName; }
            set
            {
                User.UserName = value;
                OnPropertyChanged("UserName");
            }
        }
        public string Hash
        {
            get { return User.Hash; }
            set
            {
                User.Hash = value;
                OnPropertyChanged("Hash");
            }
        }
        public string Salt
        {
            get { return User.Salt; }
            set
            {
                User.Salt = value;
                OnPropertyChanged("Salt");
            }
        }
        public bool IsAdministrator
        {
            get { return User.IsAdministrator; }
            set
            {
                User.IsAdministrator = value;
                OnPropertyChanged("IsAdministrator");
            }
        }
        public bool IsBlocked
        {
            get { return User.IsBlocked; }
            set
            {
                User.IsBlocked = value;
                OnPropertyChanged("IsBlocked");
            }
        }

        public PassportViewModel Passport
        {
            get { return passport; }
            set
            {
                passport = value;
                OnPropertyChanged("Passport");
            }
        }

        public BindingList<BetViewModel> Bets
        {
            get { return bets; }
            set
            {
                bets = value;
                OnPropertyChanged("Bets");
            }
        }

        public string CardId
        {
            get { return User.CardId; }
            set
            {
                User.CardId = value;
                OnPropertyChanged("CardId");
            }
        }
        public decimal Cash
        {
            get { return User.Cash; }
            set
            {
                User.Cash = value;
                OnPropertyChanged("Cash");
            }
        }
        public DateTime RegisterDate
        {
            get { return User.RegisterDate; }
            set
            {
                User.RegisterDate = value;
                OnPropertyChanged("RegisterDate");
            }
        }

        public string PhoneNumber
        {
            get { return User.PhoneNumber; }
            set
            {
                User.PhoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public string Email
        {
            get { return User.Email; }
            set
            {
                User.Email = value;
                OnPropertyChanged("Email");
            }
        }
        public byte[] Avatar
        {
            get { return User.Avatar; }
            set
            {
                User.Avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        public void PutMoney(decimal cash)
        {
            Cash += cash;
        }

        public void OutputMoney(decimal cash)
        {
            Cash -= cash;
        }

        #region Statistics

        public int TotalWins
        {
            get
            {
                return (from b in Bets
                        where b.BetState == State.Win
                        select b).Count();
            }
        }

        public int TotalDefeats
        {
            get
            {
                return (from b in Bets
                        where b.BetState == State.Defeat
                        select b).Count();
            }
        }

        public int TotalUndefined
        {
            get
            {
                return (from b in Bets
                        where b.BetState == State.Undefined
                        select b).Count();
            }
        }

        #endregion

        #region LoadAvatarCommand

        private Command.RelayCommand loadAvatarCommand;
        public ICommand LoadAvatarCommand
        {
            get
            {
                if (loadAvatarCommand == null)
                {
                    loadAvatarCommand = new Command.RelayCommand(LoadAvatar);
                }
                return loadAvatarCommand;
            }
        }

        private void LoadAvatar(object obj)
        {
            this.Avatar = ServiceManager.CallService("LoadPhoto", null) as byte[];
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
                    case "UserName":
                        result = this.UserNameValidation();
                        break;

                    case "CardId":
                        result = this.CardIdValidation();
                        break;

                    case "Passport":
                        result = this.PassportValidation();
                        break;

                    case "PhoneNumber":
                        result = this.PhoneNumberValidation();
                        break;

                    case "Email":
                        result = this.EmailValidation();
                        break;

                    case "Avatar":
                        result = this.AvatarValidation();
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
            this.UserName = default(string);
            this.CardId = default(string);
            this.Passport.Reset();
            this.Email = default(string);
            this.PhoneNumber = default(string);

            var info = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/StandartAvatar.png"));
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);

            Avatar = memoryStream.ToArray();
        }

        public bool IsValid()
        {
            var UserNameValid = this.UserNameValidation();
            var CardIdValid = this.CardIdValidation();
            var PassportValid = this.PassportValidation();
            var PhoneNumberValid = this.PhoneNumberValidation();
            var EmailValid = this.EmailValidation();
            var CashValid = this.CashValidation();
            var AvatarValid = this.AvatarValidation();

            var result = UserNameValid == null
                && CardIdValid == null
                && PassportValid == null
                && PhoneNumberValid == null
                && EmailValid == null
                && CashValid == null
                && AvatarValid == null;
            
            return result;
        }
        private string UserNameValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Zа-яА-Я0-9_]{2,32}$");
            
            if (string.IsNullOrEmpty(this.UserName))
                result = "Please enter an User Name";
            else if (!rule.IsMatch(this.UserName))
                result = "Incorrect User Name";

            return result;
        }

        private string CardIdValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[0-9]{16}$");
            
            if (string.IsNullOrEmpty(this.CardId))
                result = "Please enter a Card Id";
            else if (!rule.IsMatch(this.CardId))
                result = "Incorrect Card Id";
            
            return result;
        }

        private string PassportValidation()
        {
            string result = null;

            if (Passport == null || !Passport.IsValid())
                result = "Incorrect Passport";
            
            return result;
        }

        private string CashValidation()
        {
            string result = null;

            if (this.Cash < 0)
                result = "Incorrect Cash";

            return result;
        }

        private string PhoneNumberValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[+]?\d{1,3}[-\s]?\d{1,3}[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$");
            
            if (string.IsNullOrEmpty(this.PhoneNumber))
                result = "Please enter a Phone Number";
            else if (!rule.IsMatch(this.PhoneNumber))
                result = "Incorrect phone number";

            return result;
        }

        private string EmailValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            
            if (string.IsNullOrEmpty(this.Email))
                result = "Please enter a Email";
            else if (!rule.IsMatch(this.Email))
                result = "Incorrect Email";
            
            return result;
        }

        private string AvatarValidation()
        {
            string result = null;

            if (Avatar == null)
                result = "Incorrect Avatar";

            return result;
        }
        #endregion
    }
}

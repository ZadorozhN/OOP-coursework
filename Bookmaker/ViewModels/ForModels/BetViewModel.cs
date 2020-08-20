using Bookmaker.Config;
using Bookmaker.Models;
using Bookmaker.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    [Table("Bets")]
    class BetViewModel : ViewModelBase, IDataErrorInfo
    {
        private Bet Bet;
        private MatchViewModel match;
        private UserViewModel user;
        public BetViewModel(UserViewModel user, MatchViewModel match = null)
        {
            this.Bet = new Bet(0, DateTime.MinValue, 0, 0, 0);
            this.user = user;
            this.match = match;
        }
        public BetViewModel()
        {
            this.Bet = new Bet(0, DateTime.MinValue, 0, 0, 0);
        }
        public int Id
        {
            get { return Bet.Id; }
            set
            {
                Bet.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public MatchViewModel Match
        {
            get { return match; }
            set
            {
                match = value;
                OnPropertyChanged("Match");
            }
        }
        public UserViewModel User
        {
            get { return user; }
            set
            {
                user = value;
                OnPropertyChanged("User");
            }
        }
        public DateTime DateOfBet
        {
            get { return Bet.DateOfBet; }
            set
            {
                Bet.DateOfBet = value;
                OnPropertyChanged("DateOfBet");
            }
        }
        public decimal Cash
        {
            get { return Bet.Cash; }
            set
            {
                Bet.Cash = value;
                OnPropertyChanged("Cash");
            }
        }
        public MatchResult BetOn
        {
            get { return Bet.BetOn; }
            set
            {
                Bet.BetOn = value;
                OnPropertyChanged("BetOn");
            }
        }
        public State BetState
        {
            get { return Bet.BetState; }
            set
            {
                Bet.BetState = value;
                OnPropertyChanged("BetState");
            }
        }
        public IList<MatchResult> MatchResults
        {
            get
            {
                return Enum.GetValues(typeof(MatchResult)).Cast<MatchResult>().ToList<MatchResult>();
            }
        }
        public IList<State> BetStates
        {
            get
            {
                return Enum.GetValues(typeof(State)).Cast<State>().ToList<State>();
            }
        }

        public void AnnounceBetResult(MatchResult betOn, decimal coef)
        {
            if (this.BetOn == betOn)
            {
                this.BetState = State.Win;
                this.User.Cash += this.Cash * coef;
                ServiceManager.CallService("SendEmail", (this.User.Email, "Ваша ставка " + this.Match + " была выиграна \n Итого +" + this.Cash*coef, "Ставка выйграна"));
                return;
            }
            this.BetState = State.Defeat;

            ServiceManager.CallService("SendEmail", (this.User.Email, "Ваша ставка " + this.Match + " была проиграна \n Итого -" + this.Cash,"Ставка проиграна"));

        }

        public void ReturnCash()
        {
            User.Cash += Cash;
            Cash = 0;
            User.Bets.Remove(this);
            Match.BetsOnThisMatch.Remove(this);
        }

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

                    case "BetOn":
                        result = this.BetOnValidation();
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
            this.Match = default(MatchViewModel);
            this.BetOn = MatchResult.Undefined;
            this.BetState = State.Undefined;
            this.DateOfBet = DateTime.MinValue;

        }

        public bool IsValid()
        {
            var CashValid = this.CashValidation();
            var BetOnValid = this.BetOnValidation();

            var result = CashValid == null && BetOnValid == null;

            return result;
        }
        private string CashValidation()
        {
            string result = null;

            if (this.Cash == 0)
                result = "Please enter a cash";
            else if (this.Cash < 0)
                result = "Please enter a valid cash";
            else if (this.Cash > this.user.Cash)
                result = "not enough money";

            return result;
        }
        private string BetOnValidation()
        {
            string result = null;

            if (this.BetOn == MatchResult.Undefined)
                result = "Please choise a team";

            return result;
        }


        #endregion
    }
}

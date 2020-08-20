using Bookmaker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.ViewModels
{
    [Table("Matches")]
    class MatchViewModel : ViewModelBase, IDataErrorInfo
    {
        private Match Match;
        private BindingList<BetViewModel> betsOnThisMatch;
        private BindingList<ContractViewModel> contracts;
        public event Action KindOsSportChanged;
        public event Action ResultChanged;
        public MatchViewModel()
        {
            betsOnThisMatch = new BindingList<BetViewModel>();
            betsOnThisMatch.ListChanged += CoefficientChange;
            this.Match = new Match(0,DateTime.Now,0,0);
            contracts = new BindingList<ContractViewModel>();
            contracts.ListChanged += CoefficientChange;
        }

        private void CoefficientChange(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("CoefficientOnFirstTeam");
            OnPropertyChanged("CoefficientOnSecondTeam");
        }
        public BindingList<ContractViewModel> Contracts
        {
            get { return contracts; }
            set
            {
                contracts = value;
                OnPropertyChanged("Contracts");
            }
        }
        [NotMapped]
        public TeamViewModel FirstTeam
        {
            get 
            {
                var query = (from c in Contracts
                             where c.TeamNumber == ContractViewModel.TeamNumberEnum.First
                             select c);
                if(query.Count() == 0)
                {
                    Contracts.Add(new ContractViewModel(this, new TeamViewModel(), ContractViewModel.TeamNumberEnum.First));
                }
                return (from c in Contracts
                        where c.TeamNumber == ContractViewModel.TeamNumberEnum.First
                        select c).FirstOrDefault().Team;
            }
            set
            {
                (from c in Contracts
                 where c.TeamNumber == ContractViewModel.TeamNumberEnum.First
                 select c).FirstOrDefault().Team = value;
                OnPropertyChanged("FirstTeam");
            }
        }
        [NotMapped]
        public TeamViewModel SecondTeam
        {
            get
            {
                var query = (from c in Contracts
                             where c.TeamNumber == ContractViewModel.TeamNumberEnum.Second
                             select c);
                if (query.Count() == 0)
                {
                    Contracts.Add(new ContractViewModel(this, new TeamViewModel(), ContractViewModel.TeamNumberEnum.Second));
                }
                return (from c in Contracts
                        where c.TeamNumber == ContractViewModel.TeamNumberEnum.Second
                        select c).FirstOrDefault().Team;
            }
            set
            {
                (from c in Contracts
                 where c.TeamNumber == ContractViewModel.TeamNumberEnum.Second
                 select c).First().Team = value;
                OnPropertyChanged("SecondTeam");
            }
        }
        public int Id
        {
            get { return Match.Id; }
            set
            {
                Match.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public DateTime StartDate
        {
            get { return Match.StartDate; }
            set
            {
                Match.StartDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        public BindingList<BetViewModel> BetsOnThisMatch
        {
            get 
            {
                return betsOnThisMatch;
            }
            set
            {
                betsOnThisMatch = value;
                OnPropertyChanged("BetsOnThisMatch");
            }
        }
        public MatchResult Result
        {
            get { return Match.Result; }
            set
            {
                Match.Result = value;
                OnPropertyChanged("Result");
                foreach (var c in contracts)
                {
                    c.Team.WinRateChangeFromMatches();
                }
                ResultChanged?.Invoke();
            }
        }
        public KindOfSport KindOfSport
        {
            get { return Match.KindOfSport; }
            set
            {
                Match.KindOfSport = value;
                OnPropertyChanged("KindOfSport");
                KindOsSportChanged?.Invoke();
            }
        }

        #region Coefficients

        [NotMapped]
        public decimal CoefficientOnFirstTeam
        {
            get{
                decimal CashOnFT = 0;
                decimal CashOnST = 0;
                foreach (var bet in this.BetsOnThisMatch)
                {
                    if (bet.BetOn == MatchResult.FirstTeamWin)
                    {
                        CashOnFT += bet.Cash;
                    }
                    else
                    {
                        CashOnST += bet.Cash;
                    }
                }
                if (CashOnFT != 0)
                    return 1 + CashOnST / CashOnFT;
                else
                    return 1; 
            }
        }
        [NotMapped]
        public decimal CoefficientOnSecondTeam
        {
            get
            {
                decimal CashOnFT = 0;
                decimal CashOnST = 0;
                foreach (var bet in this.BetsOnThisMatch)
                {
                    if (bet.BetOn == MatchResult.FirstTeamWin)
                    {
                        CashOnFT += bet.Cash;
                    }
                    else
                    {
                        CashOnST += bet.Cash;
                    }
                }
                if (CashOnST != 0)
                    return 1 + CashOnFT / CashOnST;
                else
                    return 1;
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
                    case "FirstTeam":
                        result = this.FirstTeamValidation();
                        break;

                    case "SecondTeam":
                        result = this.SecondTeamValidation();
                        break;

                    case "StartDate":
                        result = this.StartDateValidation();
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
            betsOnThisMatch = new BindingList<BetViewModel>();
            betsOnThisMatch.ListChanged += CoefficientChange;
            Result = MatchResult.Undefined;
            KindOfSport = KindOfSport.UFC;
            StartDate = DateTime.Now;
            FirstTeam.Reset();
            SecondTeam.Reset();
        }

        public bool IsValid()
        {
            var FirstTeamValid = this.FirstTeamValidation();
            var SecondTeamValid = this.SecondTeamValidation();
            var StartDateValid = this.StartDateValidation();

            var result = FirstTeamValid == null && SecondTeamValid == null && StartDateValid == null;

            return result;
        }
        private string FirstTeamValidation()
        {
            string result = null;

            if (FirstTeam == null || !FirstTeam.IsValid() || FirstTeam.KindOfSport != KindOfSport)
                result = "Incorrect First Team";

            return result;
        }

        private string SecondTeamValidation()
        {
            string result = null;

            if (SecondTeam == null || !SecondTeam.IsValid() || SecondTeam.KindOfSport != KindOfSport)
                result = "Incorrect Second Team";
            else if (SecondTeam == FirstTeam)
                result = "Two identical teams";
            else if (FirstTeam == null || SecondTeam.KindOfSport != FirstTeam.KindOfSport)
                result = "Kinds of sport are different";

            return result;
        }

        private string StartDateValidation()
        {
            string result = null;

            if (StartDate.Year < 2000)
                result = "Incorrect Start Date";

            return result;
        }
        #endregion

        public IList<MatchResult> MatchResults
        {
            get
            {
                return Enum.GetValues(typeof(MatchResult)).Cast<MatchResult>().ToList<MatchResult>();
            }
        }
        public IList<KindOfSport> KindsOfSport
        {
            get
            {
                return Enum.GetValues(typeof(KindOfSport)).Cast<KindOfSport>().ToList<KindOfSport>();
            }
        }
        public override string ToString()
        {
            return FirstTeam.TeamName + " VS " + SecondTeam.TeamName;
        }
    }
}

using Bookmaker.Models;
using Bookmaker.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    [Table("Teams")]
    class TeamViewModel : ViewModelBase, IDataErrorInfo
    {
        private Team team;
        private BindingList<ContractViewModel> contracts;
        public TeamViewModel()
        {
            team = new Team(null, 0);
            contracts = new BindingList<ContractViewModel>();
            contracts.ListChanged += WinRateChange;
        }
        private void WinRateChange(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("WinRate");
        }
        public void WinRateChangeFromMatches()
        {
            OnPropertyChanged("WinRate");
        }
        [Key]
        public string TeamName
        {
            get { return team.TeamName; }
            set
            {
                team.TeamName = value;
                OnPropertyChanged("TeamName");
            }
        }
        public KindOfSport KindOfSport
        {
            get { return team.KindOfSport; }
            set
            {
                team.KindOfSport = value;
                OnPropertyChanged("KindOfSport");
            }
        }
        public BindingList<ContractViewModel> Contracts
        {
            get
            {
                return contracts;
            }
            set
            {
                contracts = value;
                OnPropertyChanged("Contracts");
            }
        }
        [NotMapped]
        public double WinRate
        {
            get
            {
                double winRate;
                int wins = (from m in Contracts
                            where m.Match.FirstTeam == this && m.Match.Result == MatchResult.FirstTeamWin
                            || m.Match.SecondTeam == this && m.Match.Result == MatchResult.SecondTeamWin
                            select m).Count();
                int defeats = (from m in Contracts
                               where m.Match.FirstTeam == this && m.Match.Result == MatchResult.SecondTeamWin
                               || m.Match.SecondTeam == this && m.Match.Result == MatchResult.FirstTeamWin
                               select m).Count();
                if (wins == 0)
                    return 0;
                else if (defeats == 0)
                    return 1;
                else winRate = (double)wins / (double)(wins + defeats);

                return winRate;
            }
        }
        public byte[] Logotype
        {
            get { return team.Logotype; }
            set
            {
                team.Logotype = value;
                OnPropertyChanged("Logotype");
            }
        }

        #region LoadLogotypeCommand

        private Command.RelayCommand loadLogotypeCommand;
        public ICommand LoadLogotypeCommand
        {
            get
            {
                if (loadLogotypeCommand == null)
                {
                    loadLogotypeCommand = new Command.RelayCommand(LoadLogotype);
                }
                return loadLogotypeCommand;
            }
        }

        private void LoadLogotype(object obj)
        {
            this.Logotype = ServiceManager.CallService("LoadPhoto", null) as byte[];
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
                    case "TeamName":
                        result = this.TeamNameValidation();
                        break;

                    case "Logotype":
                        result = this.TeamNameValidation();
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
            this.TeamName = default(string);
            this.KindOfSport = 0;

            var info = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/NoLogo.png"));
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);

            Logotype = memoryStream.ToArray();
        }

        public bool IsValid()
        {
            var TeamNameValid = this.TeamNameValidation();
            var LogotypeValid = this.LogotypeValidation();

            var result = TeamNameValid == null && LogotypeValid == null;

            return result;
        }
        private string TeamNameValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Zа-яА-Я0-9_]{2,64}$");

            if (string.IsNullOrEmpty(this.TeamName))
                result = "Please enter an Team Name";
            else if (!rule.IsMatch(this.TeamName))
                result = "Incorrect Team Name";

            return result;
        }

        private string LogotypeValidation()
        {
            string result = null;

            if (Logotype == null)
                result = "Incorrect Logotype";

            return result;
        }
        #endregion

        public IList<KindOfSport> KindsOfSport
        {
            get
            {
                return Enum.GetValues(typeof(KindOfSport)).Cast<KindOfSport>().ToList<KindOfSport>();
            }
        }
        public override string ToString()
        {
            return TeamName;
        }
    }
}

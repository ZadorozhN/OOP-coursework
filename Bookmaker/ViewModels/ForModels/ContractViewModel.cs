using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.ViewModels
{
    [Table("Contracts")]
    class ContractViewModel : ViewModelBase
    {
        public enum TeamNumberEnum { First = 1, Second = 2} 
        private int id;
        private MatchViewModel match;
        private TeamViewModel team;
        private TeamNumberEnum teamNumber;
        public ContractViewModel()
        {
            match = null;
            team = null;
        }
        public ContractViewModel(MatchViewModel match, TeamViewModel team,TeamNumberEnum teamNumber)
        {
            this.match = match;
            this.team = team;
            this.teamNumber = teamNumber;
        }
        [Key]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public TeamNumberEnum TeamNumber
        {
            get { return teamNumber; }
            set
            {
                teamNumber = value;
                OnPropertyChanged("TeamNumber");
            }
        }
        public MatchViewModel Match
        {
            get
            {
                return match;
            }
            set
            {
                match = value;
                OnPropertyChanged("Match");
            }
        }
        public TeamViewModel Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                OnPropertyChanged("Team");
            }
        }
    }
}

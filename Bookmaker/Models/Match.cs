using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Models
{
    class Match
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public MatchResult Result { get; set; }
        public KindOfSport KindOfSport { get; set; }
        public Match(int id, DateTime startDate,MatchResult result,KindOfSport kindOfSport)
        {
            this.Id = id;
            this.StartDate = startDate;
            this.Result = result;
            this.KindOfSport = kindOfSport;
        }
    }
}

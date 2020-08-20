using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Models
{
    class Bet
    {
        public int Id { get; set; }
        public DateTime DateOfBet { get; set; }
        public decimal Cash { get; set; }
        public MatchResult BetOn { get; set; }
        public State BetState { get; set; }
        public Bet(int id, DateTime DateOfBet, decimal cash, MatchResult betOn, State state)
        {
            this.Id = id;
            this.DateOfBet = DateOfBet;
            this.Cash = cash;
            this.BetOn = betOn;
            this.BetState = state;
        }
    }
}

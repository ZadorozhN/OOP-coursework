using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Repositories
{
    class BetRepository : IRepository<BetViewModel>
    {
        private BookmakerContext context;
        public BetRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public BetViewModel Get(object id)
        {
            return context.Bets.Find(id);
        }

        public IEnumerable<BetViewModel> GetAll()
        {
            return context.Bets;
        }

        public void Create(BetViewModel item)
        {
            context.Bets.Add(item);
        }

        public void Delete(object id)
        {
            BetViewModel bet = context.Bets.Find(id);
            if (bet != null)
            {
                context.Entry(bet).State = System.Data.Entity.EntityState.Deleted;
                context.Bets.Remove(bet);
            }
        }
        public void Update(BetViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

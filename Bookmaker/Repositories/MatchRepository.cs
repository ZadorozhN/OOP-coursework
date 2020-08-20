using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Repositories
{
    class MatchRepository : IRepository<MatchViewModel>
    {
        private BookmakerContext context;
        public MatchRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public MatchViewModel Get(object id)
        {
            return context.Matches.Find(id);
        }

        public IEnumerable<MatchViewModel> GetAll()
        {
            return context.Matches;
        }

        public void Create(MatchViewModel item)
        {
            context.Matches.Add(item);
        }

        public void Delete(object id)
        {
            MatchViewModel match = context.Matches.Find(id);
            if (match != null)
            {
                context.Entry(match).State = System.Data.Entity.EntityState.Deleted;
                context.Matches.Remove(match);
            }

        }
        public void Update(MatchViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

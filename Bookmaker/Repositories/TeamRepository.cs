using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Repositories
{
    class TeamRepository : IRepository<TeamViewModel>
    {
        private BookmakerContext context;
        public TeamRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public TeamViewModel Get(object id)
        {
            return context.Teams.Find(id);
        }

        public IEnumerable<TeamViewModel> GetAll()
        {
            return context.Teams;
        }

        public void Create(TeamViewModel item)
        {
            context.Teams.Add(item);
        }

        public void Delete(object id)
        {
            TeamViewModel team = context.Teams.Find(id);
            if (team != null)
            {
                context.Entry(team).State = System.Data.Entity.EntityState.Deleted;
                context.Teams.Remove(team);
            }

        }
        public void Update(TeamViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

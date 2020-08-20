using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Repositories
{
    class PassportRepository : IRepository<PassportViewModel>
    {
        private BookmakerContext context;
        public PassportRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public PassportViewModel Get(object id)
        {
            return context.Passports.Find(id);
        }

        public IEnumerable<PassportViewModel> GetAll()
        {
            return context.Passports;
        }

        public void Create(PassportViewModel item)
        {
            context.Passports.Add(item);
        }

        public void Delete(object id)
        {
            PassportViewModel passport = context.Passports.Find(id);
            if (passport != null)
            {
                context.Entry(passport).State = System.Data.Entity.EntityState.Deleted;
                context.Passports.Remove(passport);
            }

        }
        public void Update(PassportViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}


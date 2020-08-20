using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Repositories
{
    class ContractRepository : IRepository<ContractViewModel>
    {
        private BookmakerContext context;
        public ContractRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public ContractViewModel Get(object id)
        {
            return context.Contracts.Find(id);
        }

        public IEnumerable<ContractViewModel> GetAll()
        {
            return context.Contracts;
        }

        public void Create(ContractViewModel item)
        {
            context.Contracts.Add(item);
        }

        public void Delete(object id)
        {
            ContractViewModel contract = context.Contracts.Find(id);
            if (contract != null)
            {
                context.Entry(contract).State = System.Data.Entity.EntityState.Deleted;
                context.Contracts.Remove(contract);
            }

        }
        public void Update(ContractViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

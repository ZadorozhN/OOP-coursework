using Bookmaker.Interfaces;
using Bookmaker.ViewModels;
using System.Collections.Generic;

namespace Bookmaker.Repositories
{
    class UserRepository : IRepository<UserViewModel>
    {
        private BookmakerContext context;
        public UserRepository(BookmakerContext bookmakerContext)
        {
            context = bookmakerContext;
        }
        public UserViewModel Get(object id)
        {
            return context.Users.Find(id);
        }

        public IEnumerable<UserViewModel> GetAll()
        {
            return context.Users;
        }

        public void Create(UserViewModel item)
        {
            context.Users.Add(item);
        }

        public void Delete(object id)
        {
            UserViewModel user = context.Users.Find(id);
            if(user != null)
            {
                context.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                context.Users.Remove(user);
            }

        }
        public void Update(UserViewModel item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

using System;

namespace Bookmaker.Repositories
{
    class UnitOfWork : IDisposable
    {
        private BookmakerContext context;
        private UserRepository userRepository;
        private PassportRepository passportRepository;
        private BetRepository betRepository;
        private MatchRepository matchRepository;
        private TeamRepository teamRepository;
        private ContractRepository contractRepository;
        public UnitOfWork()
        {
            context = new BookmakerContext();
        }
        public UserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(context);
                return userRepository;
            }
        }
        public PassportRepository Passports
        {
            get
            {
                if (passportRepository == null)
                    passportRepository = new PassportRepository(context);
                return passportRepository;
            }
        }
        public BetRepository Bets
        {
            get
            {
                if (betRepository == null)
                    betRepository = new BetRepository(context);
                return betRepository;
            }
        }
        public MatchRepository Matches
        {
            get
            {
                if (matchRepository == null)
                    matchRepository = new MatchRepository(context);
                return matchRepository;
            }
        }
        public TeamRepository Teams
        {
            get
            {
                if (teamRepository == null)
                    teamRepository = new TeamRepository(context);
                return teamRepository;
            }
        }
        public ContractRepository Contracts
        {
            get
            {
                if (contractRepository == null)
                    contractRepository = new ContractRepository(context);
                return contractRepository;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public void Refresh()
        {
            context.Refresh(null);
            context.Refresh(null);
        }
        public void FullRefresh()
        {
            context.FullRefresh(null);
            context.FullRefresh(null);
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

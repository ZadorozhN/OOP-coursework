using Bookmaker.Models;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker
{
    class BookmakerContext : DbContext
    {
        public BookmakerContext()
            : base("DbConnection")
        { }

        public DbSet<UserViewModel> Users { get; set; }
        public DbSet<PassportViewModel> Passports { get; set; }
        public DbSet<MatchViewModel> Matches { get; set; }
        public DbSet<BetViewModel> Bets { get; set; }
        public DbSet<TeamViewModel> Teams { get; set; }
        public DbSet<ContractViewModel> Contracts { get; set; }

        public void Refresh(object obj)
        {
            foreach (var entity in this.ChangeTracker.Entries())
            {
                try
                {
                    entity.Reload();
                }
                catch
                {
                    MessageBox.Show("Fixed");
                    if (entity.Entity is ContractViewModel)
                        Contracts.Remove((ContractViewModel)(entity.Entity));
                    if (entity.Entity is TeamViewModel)
                        Teams.Remove((TeamViewModel)(entity.Entity));
                }
            }
            Teams.Load();
            Matches.Load();
            Contracts.Load();
            Bets.Load();
        }

        public void FullRefresh(object obj)
        {
            foreach (var entity in this.ChangeTracker.Entries())
            {
                try
                {
                    entity.Reload();
                }
                catch
                {
                    MessageBox.Show("Fixed");
                    if(entity.Entity is ContractViewModel)
                        Contracts.Remove((ContractViewModel)(entity.Entity));
                    if (entity.Entity is TeamViewModel)
                        Teams.Remove((TeamViewModel)(entity.Entity));
                }
            }
            Teams.Load();
            Matches.Load();
            Contracts.Load();
            Bets.Load();
            Passports.Load();
            Users.Load();
        }
    }
}

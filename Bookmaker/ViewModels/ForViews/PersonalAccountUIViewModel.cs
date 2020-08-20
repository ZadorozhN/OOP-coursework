using Bookmaker.Interfaces;
using Bookmaker.Models;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class PersonalAccountUIViewModel : ViewModelBase, IPageViewModel
    {
        public UnitOfWork BC { get; set; }
        public UserViewModel activeUser;
        public UserViewModel ActiveUser
        {
            get
            {
                return activeUser;
            }
            set
            {
                activeUser = value;
                OnPropertyChanged("ActiveUser");
            }
        }
        public BetViewModel activeBet;
        public BetViewModel ActiveBet
        {
            get
            {
                return activeBet;
            }
            set
            {
                activeBet = value;
                OnPropertyChanged("ActiveBet");
            }
        }
        public BindingList<MatchViewModel> matches;
        public BindingList<MatchViewModel> Matches
        {
            get
            {
                return matches;
            }
            set
            {
                matches = value;
                OnPropertyChanged("Matches");
                OnPropertyChanged("UndefinedMatches");
            }
        }
        public BindingList<TeamViewModel> teams;
        public BindingList<TeamViewModel> Teams
        {
            get
            {
                return teams;
            }
            set
            {
                teams = value;
                OnPropertyChanged("Teams");
            }
        }
        public BindingList<MatchViewModel> UndefinedMatches
        {
            get
            {
                return new BindingList<MatchViewModel>(Matches.Where(m => m.Result == MatchResult.Undefined).Select(m => m).ToList());
            }
        }

        #region Constructor
        public PersonalAccountUIViewModel(UnitOfWork bc)
        {
            BC = bc;
            Matches = ((DbSet<MatchViewModel>)(BC.Matches.GetAll())).Local.ToBindingList();
            Teams = ((DbSet<TeamViewModel>)(BC.Teams.GetAll())).Local.ToBindingList();
        }

        #endregion

        #region Refresh

        private Command.RelayCommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new Command.RelayCommand((object obj) =>
                    {
                        if (AccountCheck())
                        {
                            BC.Refresh();
                            OnPropertyChanged("UndefinedMatches");
                        }
                        else
                        {
                            ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
                        }
                    });
                }
                return refreshCommand;
            }
        }

        #endregion

        #region MakeABet

        private Command.RelayCommand makeABetCommand;
        public ICommand MakeABetCommand
        {
            get
            {
                if (makeABetCommand == null)
                {
                    makeABetCommand = new Command.RelayCommand(MakeABet,CanMakeABet);
                }
                return makeABetCommand;
            }
        }
        private void MakeABet(object obj)
        {
            BC.Refresh();

            if (AccountCheck())
            {

                MatchViewModel match = obj as MatchViewModel;
                if (CanMakeABet(obj) && match.Result == MatchResult.Undefined)
                {
                    IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                                       where m.Id == match.Id
                                                       select m;
                    if (query.Count() != 0)
                    {
                        ActiveBet.Match = match;
                        ActiveBet.DateOfBet = DateTime.Now;
                        ActiveUser.Cash -= ActiveBet.Cash;

                        BC.Bets.Create(ActiveBet);
                        BC.Save();
                        BC.Refresh();
                        ActiveBet = new BetViewModel(ActiveUser, null);
                    }
                    else
                    {
                        ServiceManager.CallService("ShowNotifyBox", Resources.MatchDeleted);
                    }
                }
                else
                {
                    ServiceManager.CallService("ShowNotifyBox", Resources.MatchResultAnnounced);
                }
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }
        private bool CanMakeABet(object obj)
        {
            MatchViewModel match = obj as MatchViewModel;
            return match != null && ActiveBet.IsValid() && match.Result == MatchResult.Undefined; 
        }

        #endregion

        #region CancelBetCommand

        private Command.RelayCommand cancelBetCommand;
        public ICommand CancelBetCommand
        {
            get
            {
                if (cancelBetCommand == null)
                {
                    cancelBetCommand = new Command.RelayCommand(CancelBet, CanCancelBet);
                }
                return cancelBetCommand;
            }
        }

        private void CancelBet(object obj)
        {
            BC.Refresh();

            if (AccountCheck())
            {


                BetViewModel bet = obj as BetViewModel;


                IQueryable<BetViewModel> query = from b in ((DbSet<BetViewModel>)(BC.Bets.GetAll()))
                                                 where b.Id == bet.Id
                                                 select b;

                if (CanCancelBet(obj) && query.Count() != 0)
                {
                    bet.ReturnCash();
                    BC.Bets.Delete(bet.Id);
                    BC.Save();
                    BC.Refresh();
                }
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }
        private bool CanCancelBet(object obj)
        {
            BetViewModel bet = obj as BetViewModel;
            if (bet != null && bet.BetState == State.Undefined)
                return true;
            return false;
        }

        #endregion

        #region GoToSettingsUIViewCommand

        private Command.RelayCommand goToSettingsUIViewCommand;
        public ICommand GoToSettingsUIViewCommand
        {
            get
            {
                if (goToSettingsUIViewCommand == null)
                {
                    goToSettingsUIViewCommand = new Command.RelayCommand(GoToSettingsUIView);
                }
                return goToSettingsUIViewCommand;
            }
        }
        private void GoToSettingsUIView(object obj)
        {
            if (AccountCheck())
            {
                Mediator.Mediator.Notify("GoToSettingsUIScreen", ActiveUser);
                this.ActiveUser = null;
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        #endregion

        #region GoToUserDataUIViewCommand

        private Command.RelayCommand goToUserDataUIViewCommand;
        public ICommand GoToUserDataUIViewCommand
        {
            get
            {
                if (goToUserDataUIViewCommand == null)
                {
                    goToUserDataUIViewCommand = new Command.RelayCommand(GoToUserDataUIView);
                }
                return goToUserDataUIViewCommand;
            }
        }
        private void GoToUserDataUIView(object obj)
        {
            if (AccountCheck())
            {
                Mediator.Mediator.Notify("GoToUserDataUIScreen", ActiveUser);
                this.ActiveUser = null;
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        #endregion

        #region GoToPutMoneyUIViewCommand

        private Command.RelayCommand goToPutMoneyUIViewCommand;
        public ICommand GoToPutMoneyUIViewCommand
        {
            get
            {
                if (goToPutMoneyUIViewCommand == null)
                {
                    goToPutMoneyUIViewCommand = new Command.RelayCommand(GoToPutMoneyUIView);
                }
                return goToPutMoneyUIViewCommand;
            }
        }
        private void GoToPutMoneyUIView(object obj)
        {
            if (AccountCheck())
            {
                Mediator.Mediator.Notify("GoToPutMoneyUIScreen", ActiveUser);
                this.ActiveUser = null;
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.YourAccountDeleted);
            }
        }

        #endregion

        #region LogOut

        private Command.RelayCommand logOutCommand;
        public ICommand LogOutCommand
        {
            get
            {
                if (logOutCommand == null)
                {
                    logOutCommand = new Command.RelayCommand(LogOut);
                }
                return logOutCommand;
            }
        }
        private void LogOut(object obj)
        {
            this.ActiveUser = null;
            this.ActiveBet = null;
            Mediator.Mediator.Notify("GoToLoginUIScreen", "");
        }

        #endregion

        #region AllMatchesCommand

        private Command.RelayCommand allMatchesCommand;
        public ICommand AllMatchesCommand
        {
            get
            {
                if (allMatchesCommand == null)
                {
                    allMatchesCommand = new Command.RelayCommand(AllMatchesFilter);
                }
                return allMatchesCommand;
            }
        }

        private void AllMatchesFilter(object obj)
        {
            IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                               select m;

            Matches = new BindingList<MatchViewModel>(query.ToList());
        }

        #endregion

        #region OnlyUFCMatchesCommand

        private Command.RelayCommand onlyUFCMatchesCommand;
        public ICommand OnlyUFCMatchesCommand
        {
            get
            {
                if (onlyUFCMatchesCommand == null)
                {
                    onlyUFCMatchesCommand = new Command.RelayCommand(OnlyUFCMatchesFilter);
                }
                return onlyUFCMatchesCommand;
            }
        }

        private void OnlyUFCMatchesFilter(object obj)
        {
            IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                               where m.KindOfSport == KindOfSport.UFC
                                               select m;

            Matches = new BindingList<MatchViewModel>(query.ToList());
        }

        #endregion

        #region OnlyBasketBallMatchesCommand

        private Command.RelayCommand onlyBasketBallMatchesCommand;
        public ICommand OnlyBasketBallMatchesCommand
        {
            get
            {
                if (onlyBasketBallMatchesCommand == null)
                {
                    onlyBasketBallMatchesCommand = new Command.RelayCommand(OnlyBasketBallMatchesFilter);
                }
                return onlyBasketBallMatchesCommand;
            }
        }

        private void OnlyBasketBallMatchesFilter(object obj)
        {
            IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                               where m.KindOfSport == KindOfSport.BasketBall
                                               select m;

            Matches = new BindingList<MatchViewModel>(query.ToList());
        }

        #endregion

        #region OnlySoccerMatchesCommand

        private Command.RelayCommand onlySoccerMatchesCommand;
        public ICommand OnlySoccerMatchesCommand
        {
            get
            {
                if (onlySoccerMatchesCommand == null)
                {
                    onlySoccerMatchesCommand = new Command.RelayCommand(OnlySoccerMatchesFilter);
                }
                return onlySoccerMatchesCommand;
            }
        }

        private void OnlySoccerMatchesFilter(object obj)
        {
            IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                               where m.KindOfSport == KindOfSport.Soccer
                                               select m;

            Matches = new BindingList<MatchViewModel>(query.ToList());
        }

        #endregion

        private bool AccountCheck()
        {
            BC.Refresh();
            IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                              where u.Id == ActiveUser.Id
                                              select u;
            if(query.Count() != 0 && !query.First().IsBlocked)
            {
                return true;
            }
            return false;
        }
    }
}

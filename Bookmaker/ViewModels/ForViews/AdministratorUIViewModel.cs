using Bookmaker.Interfaces;
using Bookmaker.Models;
using Bookmaker.Properties;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class AdministratorUIViewModel : ViewModelBase, IPageViewModel
    {
        #region Properties
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
        public UserViewModel searchedUser;
        public UserViewModel SearchedUser
        {
            get
            {
                return searchedUser;
            }
            set
            {
                searchedUser = value;
                OnPropertyChanged("SearchedUser");
            }
        }
        public TeamViewModel activeTeam;
        public TeamViewModel ActiveTeam
        {
            get
            {
                return activeTeam;
            }
            set
            {
                activeTeam = value;
                OnPropertyChanged("ActiveTeam");
            }
        }
        public MatchViewModel activeMatch;
        public MatchViewModel ActiveMatch
        {
            get
            {
                return activeMatch;
            }
            set
            {
                activeMatch = value;
                OnPropertyChanged("ActiveMatch");
            }
        }
        public MatchViewModel newMatch;
        public MatchViewModel NewMatch
        {
            get
            {
                return newMatch;
            }
            set
            {
                newMatch = value;
                OnPropertyChanged("NewMatch"); 
                OnPropertyChanged("SortedTeams");
                newMatch.KindOsSportChanged += () => OnPropertyChanged("SortedTeams");
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
        public BindingList<UserViewModel> searchResult;
        public BindingList<UserViewModel> SearchResult
        {
            get
            {
                return searchResult;
            }
            set
            {
                searchResult = value;
                OnPropertyChanged("SearchResult");
            }
        }
        public BindingList<MatchViewModel> UndefinedMatches
        {
            get
            {
                return new BindingList<MatchViewModel>(Matches.Where(m => m.Result == MatchResult.Undefined).Select(m => m).ToList());
            }
        }
        public BindingList<UserViewModel> users;
        public BindingList<UserViewModel>  Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                OnPropertyChanged("Users");
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
                OnPropertyChanged("SortedTeams");
            }
        }
        [NotMapped]
        public BindingList<TeamViewModel> SortedTeams
        {
            get
            {
                return new BindingList<TeamViewModel>((from t in Teams
                                                       where t.KindOfSport == NewMatch.KindOfSport
                                                       select t).ToList());
            }
        }
        public UnitOfWork BC { get; set; }

        #endregion

        #region Constructor
        public AdministratorUIViewModel(UnitOfWork bc)
        {
            BC = bc;
            Users = ((DbSet<UserViewModel>)(BC.Users.GetAll())).Local.ToBindingList();
            Matches = ((DbSet<MatchViewModel>)(BC.Matches.GetAll())).Local.ToBindingList();
            Teams = ((DbSet<TeamViewModel>)(BC.Teams.GetAll())).Local.ToBindingList();
            NewMatch = new MatchViewModel();
            SearchedUser = new UserViewModel();

        }
        #endregion

        #region AnnounceMatchResult

        private Command.RelayCommand announceMatchResultCommand;
        public ICommand AnnounceMatchResultCommand
        {
            get
            {
                if (announceMatchResultCommand == null)
                {
                    announceMatchResultCommand = new Command.RelayCommand(AnnounceMatchResult, CanAnnounceMatchResult);
                }
                return announceMatchResultCommand;
            }
        }
        private void AnnounceMatchResult(object obj)
        {
            BC.FullRefresh();
            MatchViewModel match = obj as MatchViewModel;
            if (match.Result == MatchResult.Undefined)
            {
                match.Result = ActiveMatch.Result;

                decimal coef;
                if (match.Result == MatchResult.FirstTeamWin)
                {
                    coef = match.CoefficientOnFirstTeam;
                }
                else
                {
                    coef = match.CoefficientOnSecondTeam;
                }

                foreach (var bet in match.BetsOnThisMatch)
                {
                    bet.AnnounceBetResult(match.Result, coef);
                }
                BC.Matches.Update(match);
                BC.Save();
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.MatchResultAnnounced);
            }
            OnPropertyChanged("UndefinedMatches");
        }
        private bool CanAnnounceMatchResult(object obj)
        {
            return obj is MatchViewModel && ActiveMatch.Result != MatchResult.Undefined;
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
            this.ActiveMatch = null;
            Mediator.Mediator.Notify("GoToLoginUIScreen", "");
        }

        #endregion

        #region FullRefresh

        private Command.RelayCommand fullRefreshCommand;
        public ICommand FullRefreshCommand
        {
            get
            {
                if (fullRefreshCommand == null)
                {
                    fullRefreshCommand = new Command.RelayCommand((object obj) =>
                    {
                        BC.FullRefresh();
                        OnPropertyChanged("UndefinedMatches");
                        OnPropertyChanged("SortedTeams");
                    });
                }
                return fullRefreshCommand;
            }
        }

        #endregion

        #region SaveUserChangesCommand

        private Command.RelayCommand saveUserChangesCommand;
        public ICommand SaveUserChangesCommand
        {
            get
            {
                if (saveUserChangesCommand == null)
                {
                    saveUserChangesCommand = new Command.RelayCommand(SaveUserChanges,CanSaveUserChanges);
                }
                return saveUserChangesCommand;
            }
        }

        private void SaveUserChanges(object obj)
        {
            UserViewModel user = obj as UserViewModel;
            BC.Users.Update(user);

            BC.Save();

            BC.FullRefresh();
        }
        private bool CanSaveUserChanges(object obj)
        {
            UserViewModel user = obj as UserViewModel;

            if (user != null)
            {
                return true && user.IsValid();
            }
            return false;
        }

        #endregion

        #region SaveTeamChangesCommand

        private Command.RelayCommand saveTeamChangesCommand;
        public ICommand SaveTeamChangesCommand
        {
            get
            {
                if (saveTeamChangesCommand == null)
                {
                    saveTeamChangesCommand = new Command.RelayCommand(SaveTeamChanges, CanSaveTeamChanges);
                }
                return saveTeamChangesCommand;
            }
        }

        private void SaveTeamChanges(object obj)
        {
            TeamViewModel team = obj as TeamViewModel;

            IQueryable<TeamViewModel> query = from t in ((DbSet<TeamViewModel>)(BC.Teams.GetAll()))
                                              where t.TeamName == team.TeamName
                                              select t;
            if (query.Count() != 0)
            {
                BC.Teams.Update(team);
                BC.Save();
            }
            BC.FullRefresh();
        }

        private bool CanSaveTeamChanges(object obj)
        {
            TeamViewModel team = obj as TeamViewModel;

            if (team != null)
            {
                return true && team.IsValid();
            }
            return false;
        }

        #endregion

        #region AddTeamCommand

        private Command.RelayCommand addTeamCommand;
        public ICommand AddTeamCommand
        {
            get
            {
                if (addTeamCommand == null)
                {
                    addTeamCommand = new Command.RelayCommand(AddTeam,CanAddTeam);
                }
                return addTeamCommand;
            }
        }

        private void AddTeam(object obj)
        {
            BC.FullRefresh();

            IQueryable<TeamViewModel> query = from t in ((DbSet<TeamViewModel>)(BC.Teams.GetAll()))
                                              where t.TeamName == ActiveTeam.TeamName
                                              select t;
            if (query.Count() == 0)
            {
                BC.Teams.Create(ActiveTeam);
                BC.Save();
                OnPropertyChanged("Teams");
                OnPropertyChanged("SortedTeams");
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.TeamExists);
            }
            ActiveTeam = new TeamViewModel();
        }

        private bool CanAddTeam(object obj)
        {
            return ActiveTeam.IsValid();
        }

        #endregion

        #region AddMatchCommand

        private Command.RelayCommand addMatchCommand;
        public ICommand AddMatchCommand
        {
            get
            {
                if (addMatchCommand == null)
                {
                    addMatchCommand = new Command.RelayCommand(AddMatch, CanAddMatch);
                }
                return addMatchCommand;
            }
        }

        private void AddMatch(object obj)
        {
            BC.FullRefresh();
            IQueryable<TeamViewModel> query1 = from t in ((DbSet<TeamViewModel>)(BC.Teams.GetAll()))
                                               where t.TeamName == NewMatch.FirstTeam.TeamName
                                               select t;

            IQueryable<TeamViewModel> query2 = from t in ((DbSet<TeamViewModel>)(BC.Teams.GetAll()))
                                               where t.TeamName == NewMatch.SecondTeam.TeamName
                                               select t;

            if (query1.Count() != 0 && query2.Count() != 0)
            {
                BC.Matches.Create(NewMatch);
                ((DbSet<ContractViewModel>)(BC.Contracts.GetAll())).AddRange(NewMatch.Contracts);
                BC.Save();
                NewMatch = new MatchViewModel();
                OnPropertyChanged("UndefinedMatches");
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Properties.Resources.TeamDeleted);
                FullRefreshCommand.Execute(null);
                NewMatch = new MatchViewModel();
            }
        }

        private bool CanAddMatch(object obj)
        {
            return NewMatch.IsValid();
        }

        #endregion

        #region DeleteMatchCommand

        private Command.RelayCommand deleteMatchCommand;
        public ICommand DeleteMatchCommand
        {
            get
            {
                if (deleteMatchCommand == null)
                {
                    deleteMatchCommand = new Command.RelayCommand(DeleteMatch, CanDeleteMatch);
                }
                return deleteMatchCommand;
            }
        }

        private void DeleteMatch(object obj)
        {
            BC.FullRefresh();
            MatchViewModel match = obj as MatchViewModel;

            IQueryable<MatchViewModel> query = from m in ((DbSet<MatchViewModel>)(BC.Matches.GetAll()))
                                               where m.Id == match.Id
                                               select m;

            if (CanDeleteMatch(obj) && query.Count() != 0)
            {
                foreach(var t in match.Contracts)
                {
                    t.Team.Contracts.Remove(t);
                    t.Team = null;
                    t.Match = null;
                }
                ((DbSet<ContractViewModel>)(BC.Contracts.GetAll())).RemoveRange(match.Contracts);
                BC.Matches.Delete(match.Id);
                BC.Save();
                match.Contracts = null;
                BC.FullRefresh();
            }

            OnPropertyChanged("UndefinedMatches");
        }

        private bool CanDeleteMatch(object obj)
        {
            MatchViewModel match = obj as MatchViewModel;
            if (match != null && match.BetsOnThisMatch.Count == 0)
                return true;
            return false;
        }

        #endregion

        #region DeleteTeamCommand

        private Command.RelayCommand deleteTeamCommand;
        public ICommand DeleteTeamCommand
        {
            get
            {
                if (deleteTeamCommand == null)
                {
                    deleteTeamCommand = new Command.RelayCommand(DeleteTeam, CanDeleteTeam);
                }
                return deleteTeamCommand;
            }
        }

        private void DeleteTeam(object obj)
        {
            BC.FullRefresh();
            TeamViewModel team = obj as TeamViewModel;


            IQueryable<TeamViewModel> query = from t in ((DbSet<TeamViewModel>)(BC.Teams.GetAll()))
                                              where t.TeamName == team.TeamName
                                              select t;

            if (CanDeleteTeam(obj) && query.Count() != 0)
            {
                BC.Teams.Delete(team.TeamName);
                BC.Save();
                FullRefreshCommand.Execute(null);
            }
        }
        private bool CanDeleteTeam(object obj)
        {
            TeamViewModel team = obj as TeamViewModel;
            if (team != null && team.Contracts.Count == 0)
                return true;
            return false;
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
            BC.FullRefresh();
            BetViewModel bet = obj as BetViewModel;


            IQueryable<BetViewModel> query = from b in ((DbSet<BetViewModel>)(BC.Bets.GetAll()))
                                             where b.Id == bet.Id
                                             select b;

            if (CanCancelBet(obj) && query.Count() != 0)
            {
                bet.ReturnCash();
                BC.Bets.Delete(bet.Id);
                BC.Save();
                BC.FullRefresh();
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

        #region SearchByUserNameCommand

        private Command.RelayCommand searchByUserNameCommand;
        public ICommand SearchByUserNameCommand
        {
            get
            {
                if (searchByUserNameCommand == null)
                {
                    searchByUserNameCommand = new Command.RelayCommand(SearchByUserName, CanSearchByUserName);
                }
                return searchByUserNameCommand;
            }
        }

        private void SearchByUserName(object obj)
        {
            string UserName = obj as string;

            if (UserName != null)
            {
                BC.FullRefresh();



                IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                                  where u.UserName == UserName
                                                  select u;
                if (query.Count() != 0)
                {
                    GoToUserRedactorUIView((query.First()));
                }
                else
                {
                    ServiceManager.CallService("ShowNotifyBox", Resources.UserIsNotFound);
                }
            }
        }

        private bool CanSearchByUserName(object obj)
        {
            return !string.IsNullOrEmpty((obj as string));
        }

        #endregion

        #region SearchByUserNameCommand

        private Command.RelayCommand searchUserCommand;
        public ICommand SearchUserCommand
        {
            get
            {
                if (searchUserCommand == null)
                {
                    searchUserCommand = new Command.RelayCommand(SearchUser);
                }
                return searchUserCommand;
            }
        }

        private void SearchUser(object obj)
        {
            BC.FullRefresh();
            SearchResult = Users;
            if (!string.IsNullOrEmpty(SearchedUser.UserName))
            {
                SearchResult = new BindingList<UserViewModel>(SearchResult.Where(u => u.UserName.Contains(SearchedUser.UserName)).ToList());
            }
            if (!string.IsNullOrEmpty(SearchedUser.Email))
            {
                SearchResult = new BindingList<UserViewModel>(SearchResult.Where(u => u.Email.Contains(SearchedUser.Email)).ToList());
            }
            if (!string.IsNullOrEmpty(SearchedUser.Passport.FirstName))
            {
                SearchResult = new BindingList<UserViewModel>(SearchResult.Where(u => u.Passport.FirstName.Contains(SearchedUser.Passport.FirstName)).ToList());
            }
            if (!string.IsNullOrEmpty(SearchedUser.Passport.LastName))
            {
                SearchResult = new BindingList<UserViewModel>(SearchResult.Where(u => u.Passport.FirstName.Contains(SearchedUser.Passport.LastName)).ToList());
            }
            if (!string.IsNullOrEmpty(SearchedUser.Passport.Patronymic))
            {
                SearchResult = new BindingList<UserViewModel>(SearchResult.Where(u => u.Passport.FirstName.Contains(SearchedUser.Passport.Patronymic)).ToList());
            }
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

        #region GoToUserRedactorUIViewCommand

        private Command.RelayCommand goToUserRedactorUIViewCommand;
        public ICommand GoToUserRedactorUIViewCommand
        {
            get
            {
                if (goToUserRedactorUIViewCommand == null)
                {
                    goToUserRedactorUIViewCommand = new Command.RelayCommand(GoToUserRedactorUIView, CanGoToUserRedactorUIView);
                }
                return goToUserRedactorUIViewCommand;
            }
        }
        private void GoToUserRedactorUIView(object obj)
        {
            BC.FullRefresh();


            UserViewModel user = obj as UserViewModel;

            IQueryable<UserViewModel> query = from u in ((DbSet<UserViewModel>)(BC.Users.GetAll()))
                                              where u.Id == user.Id
                                              select u;
            if (query.Count() != 0)
            {
                Mediator.Mediator.Notify("GoToUserRedactorUIScreen", (user, ActiveUser));
            }
            else
            {
                ServiceManager.CallService("ShowNotifyBox", Resources.SelectedAccountDeleted);
            }

        }

        private bool CanGoToUserRedactorUIView(object obj)
        {
            UserViewModel user = obj as UserViewModel;
            if (user != null)
            {
                return true;
            }
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
            BC.FullRefresh();
            Mediator.Mediator.Notify("GoToSettingsUIScreen", ActiveUser);
            this.ActiveUser = null;
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
            BC.FullRefresh();
            Mediator.Mediator.Notify("GoToUserDataUIScreen", ActiveUser);
            this.ActiveUser = null;
        }

        #endregion
    }
}

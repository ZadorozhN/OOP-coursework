using Bookmaker.Config;
using Bookmaker.Interfaces;
using Bookmaker.Repositories;
using Bookmaker.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Bookmaker.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public UnitOfWork BC { get; set; }

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public string Background
        {
            get { return ConfigManager.GetConfigManager.ActiveBackground; }
        }
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void OnGoLoginUIScreen(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }

        private void OnGoRegisterUIScreen(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }
        private void OnGoVerifyingUIScreen(object obj)
        {
            ChangeViewModel(PageViewModels[2]);
        }
        private void OnGoPersonalAccountUIScreen(object obj)
        {
            UserViewModel activeUser = obj as UserViewModel;
            (PageViewModels[3] as PersonalAccountUIViewModel).ActiveUser = activeUser;
            (PageViewModels[3] as PersonalAccountUIViewModel).ActiveBet = new BetViewModel(activeUser, null);

            ChangeViewModel(PageViewModels[3]);
        }
        private void OnGoAdministratorUIScreen(object obj)
        {
            UserViewModel activeUser = obj as UserViewModel;
            (PageViewModels[4] as AdministratorUIViewModel).ActiveUser = activeUser;
            (PageViewModels[4] as AdministratorUIViewModel).ActiveMatch = new MatchViewModel();
            (PageViewModels[4] as AdministratorUIViewModel).ActiveTeam = new TeamViewModel();
            (PageViewModels[4] as AdministratorUIViewModel).NewMatch = new MatchViewModel();

            ChangeViewModel(PageViewModels[4]);
        }

        private void OnGoSettingsUIScreen(object obj)
        {
            UserViewModel activeUser = obj as UserViewModel;
            (PageViewModels[5] as SettingsUIViewModel).ActiveUser = activeUser;

            ChangeViewModel(PageViewModels[5]);
        }

        private void OnGoUserDataUIScreen(object obj)
        {
            UserViewModel activeUser = obj as UserViewModel;
            (PageViewModels[6] as UserDataUIViewModel).ActiveUser = activeUser;

            ChangeViewModel(PageViewModels[6]);
        }

        private void OnGoPutMoneyUIScreen(object obj)
        {
            UserViewModel activeUser = obj as UserViewModel;
            (PageViewModels[7] as PutMoneyUIViewModel).ActiveUser = activeUser;

            ChangeViewModel(PageViewModels[7]);
        }

        private void OnGoUserRedactorUIScreen(object obj)
        {
            (UserViewModel,UserViewModel)? Users = obj as (UserViewModel,UserViewModel)?;
            (PageViewModels[8] as UserRedactorUIViewModel).ActiveUser = Users.Value.Item1;
            (PageViewModels[8] as UserRedactorUIViewModel).Administrator = Users.Value.Item2;

            ChangeViewModel(PageViewModels[8]);
        }

        #region Constructor        
        public MainViewModel()
        {
            BC = new UnitOfWork();
            BC.Refresh();

            PageViewModels.Add(new LoginUIViewModel(BC));
            PageViewModels.Add(new RegisterUIViewModel(BC));
            PageViewModels.Add(new VerifyingUIViewModel(BC));
            PageViewModels.Add(new PersonalAccountUIViewModel(BC));
            PageViewModels.Add(new AdministratorUIViewModel(BC));
            PageViewModels.Add(new SettingsUIViewModel(BC));
            PageViewModels.Add(new UserDataUIViewModel(BC));
            PageViewModels.Add(new PutMoneyUIViewModel(BC));
            PageViewModels.Add(new UserRedactorUIViewModel(BC));

            CurrentPageViewModel = PageViewModels[0];

            Mediator.Mediator.Subscribe("GoToLoginUIScreen", OnGoLoginUIScreen);
            Mediator.Mediator.Subscribe("GoToRegisterUIScreen", OnGoRegisterUIScreen);
            Mediator.Mediator.Subscribe("GoToVerifyingUIScreen", OnGoVerifyingUIScreen);
            Mediator.Mediator.Subscribe("GoToPersonalAccountUIScreen", OnGoPersonalAccountUIScreen);
            Mediator.Mediator.Subscribe("GoToAdministratorUIScreen", OnGoAdministratorUIScreen);
            Mediator.Mediator.Subscribe("GoToSettingsUIScreen", OnGoSettingsUIScreen);
            Mediator.Mediator.Subscribe("GoToUserDataUIScreen", OnGoUserDataUIScreen);
            Mediator.Mediator.Subscribe("GoToPutMoneyUIScreen", OnGoPutMoneyUIScreen);
            Mediator.Mediator.Subscribe("GoToUserRedactorUIScreen", OnGoUserRedactorUIScreen);

            ConfigManager.GetConfigManager.BackgroundChanged += () => OnPropertyChanged("Background");
        }
        #endregion

        #region CloseWindowCommand

        private Command.RelayCommand closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get
            {
                if (closeWindowCommand == null)
                {
                    closeWindowCommand = new Command.RelayCommand(CloseWindow, CanCloseWindow);
                }
                return closeWindowCommand;
            }
        }

        private void CloseWindow(object obj)
        {
            ServiceManager.CallService("CloseWindow", obj);
        }

        private bool CanCloseWindow(object obj)
        {
            return obj is Window;
        }
        #endregion

        #region CollapseWindowCommand

        private Command.RelayCommand collapseWindowCommand;
        public ICommand CollapseWindowCommand
        {
            get
            {
                if (collapseWindowCommand == null)
                {
                    collapseWindowCommand = new Command.RelayCommand(CollapseWindow, CanCollapseWindow);
                }
                return collapseWindowCommand;
            }
        }

        private void CollapseWindow(object obj)
        {
            ServiceManager.CallService("CollapseWindow", obj);
        }

        private bool CanCollapseWindow(object obj)
        {
            return obj is Window;
        }
        #endregion

        #region DragMoveCommand

        private Command.RelayCommand dragMoveCommand;
        public ICommand DragMoveCommand
        {
            get
            {
                if (dragMoveCommand == null)
                {
                    dragMoveCommand = new Command.RelayCommand(DragMove, CanDragMove);
                }
                return dragMoveCommand;
            }
        }

        private void DragMove(object obj)
        {
            (obj as Window).DragMove();
        }

        private bool CanDragMove(object obj)
        {
            return obj is Window;
        }
        #endregion
    }
}

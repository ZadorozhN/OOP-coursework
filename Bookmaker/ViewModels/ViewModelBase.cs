using System.ComponentModel;

namespace Bookmaker.ViewModels
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Images

        public string AppIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/AppIco.ico"; }
        }
        public string CloseIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/CloseIcon.png"; }
        }
        public string AddIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/add.png"; }
        }
        public string BlockIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/block.png"; }
        }
        public string HomeIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/home.png"; }
        }
        public string TrashIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/trash.png"; }
        }
        public string SettingIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/settings.png"; }
        }
        public string SearchIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/search.png"; }
        }
        public string PersonalIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/personal.png"; }
        }
        public string ImageIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/image.png"; }
        }
        public string WalletIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/wallet.png"; }
        }
        public string CardIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/card.png"; }
        }
        public string RefreshIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/refresh.png"; }
        }
        public string ReturnIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/return.png"; }
        }
        public string DoorIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/door.png"; }
        }
        public string NoDoorIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/nodoor.png"; }
        }
        public string CheckIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/check.png"; }
        }
        public string SendIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/send.png"; }
        }
        public string SaveIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/save.png"; }
        }
        public string UFCIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/UFC.png"; }
        }
        public string BasketBallIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/BasketBall.png"; }
        }
        public string SoccerIcon
        {
            get { return @"/Bookmaker;component/Resources/Icons/SoccerBall.png"; }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
                
            }
        }

    }

}

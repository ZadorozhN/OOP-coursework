using Bookmaker.Config;
using Bookmaker.MainWnd;
using Bookmaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigManager.GetConfigManager.SetAppLanguage();

            ConfigManager.GetConfigManager.SetActiveColor();

            ConfigManager.GetConfigManager.SetAppTheme();

            MainWindow app = new MainWindow();
            MainViewModel context = new MainViewModel();
            app.DataContext = context;
            app.Show();
        }
    }
}

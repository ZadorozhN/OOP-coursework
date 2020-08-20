using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.Config
{
    class ConfigManager
    {
        static private ConfigManager configManager;
        private ConfigManager()
        { }
        static public ConfigManager GetConfigManager
        {
            get
            {
                if (configManager == null)
                {
                    configManager = new ConfigManager();
                }
                return configManager;
            }
        }

        #region Language Config

        public void SetAppLanguage()
        {
            Properties.Settings.Default.Upgrade();
            string language = Properties.Settings.Default.Language;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        }
        public string GetActiveLanguage()
        {
            return Properties.Settings.Default.Language;
        }
        public void WriteLanguageConfig(string language) => WriteConfig(Properties.Settings.Default.Language = language);

        #endregion

        #region ColorChanger
        public void SetActiveColor()
        {
            bool WithAccent = Properties.Settings.Default.ActiveColor != "Grey" && Properties.Settings.Default.ActiveColor != "Brown"
                && Properties.Settings.Default.ActiveColor != "BlueGrey";

            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." +
                Properties.Settings.Default.ActiveColor + ".xaml");

            ResourceDictionary dictAccent = new ResourceDictionary();
            if (WithAccent)
            {
                dictAccent.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor." +
                Properties.Settings.Default.ActiveColor + ".xaml");
            }

            ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                          where d.Source != null && d.Source.OriginalString.Contains($"/Primary/")
                                          select d).First();

            if (oldDict != null)
            {
                int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                if (WithAccent)
                {
                    ResourceDictionary oldDictAccent = (from d in Application.Current.Resources.MergedDictionaries
                                                        where d.Source != null && d.Source.OriginalString.Contains($"/Accent/")
                                                        select d).First();

                    ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDictAccent);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDictAccent);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dictAccent);
                }
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(dict);
                if (WithAccent)
                    Application.Current.Resources.MergedDictionaries.Add(dictAccent);
            }
        }
        public void WriteColorConfig(string color) => WriteConfig(Properties.Settings.Default.ActiveColor = color);

        #endregion

        #region BackgroundChanger

        public event Action BackgroundChanged;

        public List<string> AvailableBackgrounds { get; private set; }
        public string ActiveBackground
        {
            get
            {
                return @"/Bookmaker;component/Resources/Backgrounds/" + Properties.Settings.Default.Background + @".jpg";
            }
        }
        public void WriteBackgroundConfig(string background)
        {
            WriteConfig(Properties.Settings.Default.Background = background);
            BackgroundChanged?.Invoke();
        }


        #endregion

        #region ChangeTheme

        public void SetAppTheme()
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme."+
                Properties.Settings.Default.Theme
                +".xaml");

            ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                          where d.Source != null && (d.Source.OriginalString.Contains($"Dark")
                                          || d.Source.OriginalString.Contains($"Light"))
                                          select d).First();

            if (oldDict != null)
            {
                int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                Application.Current.Resources.MergedDictionaries.Insert(ind, dict);

            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
        public string GetActiveTheme()
        {
            return Properties.Settings.Default.Theme;
        }
        public void SwapTheme()
        {
            if(GetActiveTheme() == "Light")
            {
                WriteThemeConfig("Dark");
            }
            else
            {
                WriteThemeConfig("Light");
            }
        }
        private void WriteThemeConfig(string theme) => WriteConfig(Properties.Settings.Default.Theme = theme);
        #endregion

        #region Utils

        private void WriteConfig(string value) => Properties.Settings.Default.Save();

        #endregion
    }
}

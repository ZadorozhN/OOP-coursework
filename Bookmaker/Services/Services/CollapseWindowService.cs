using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.Services
{
    class CollapseWindowService : IService
    {
        public object Execute(object obj)
        {
            if (obj is Window)
            {
                if ((obj as Window).WindowState == WindowState.Normal)
                    (obj as Window).WindowState = WindowState.Maximized;
                else
                    (obj as Window).WindowState = WindowState.Normal;
                return true;
            }
            return false;
        }
    }
}

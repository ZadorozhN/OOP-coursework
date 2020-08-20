using Bookmaker.MainWnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.Services
{
    class CloseWindowService : IService
    {
        public object Execute(object obj)
        {
            if (obj is Window)
            {
                (obj as Window).Close();
                return true;
            }
            return false;
        }
    }
}

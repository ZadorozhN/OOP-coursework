using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bookmaker.Services
{
    class ShowNotifyBoxService : IService
    {
        public object Execute(object obj)
        {
            if (obj != null)
            {
                try
                {
                    NotifyBox notifyBox = new NotifyBox();
                    var Context = new NotifyBoxContext(obj as string);
                    notifyBox.DataContext = Context;
                    notifyBox.ShowDialog();
                }
                catch
                {

                }

                return true;
            }
            return false;
        }
    }
}

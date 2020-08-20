using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Services
{
    static class ServiceManager
    {
        static Dictionary<string, IService> services;
        static ServiceManager()
        {
            services = new Dictionary<string, IService>();
            services.Add("LoadPhoto", new LoadPhotoService());
            services.Add("CloseWindow", new CloseWindowService());
            services.Add("ShowNotifyBox", new ShowNotifyBoxService());
            services.Add("SendEmail", new SendEmailService());
            services.Add("CloseApp", new CloseAppService());
            services.Add("CollapseWindow", new CollapseWindowService());
        }
        static public object CallService(string ServiceName, object ServiceParam)
        {
            try
            {
                IService service;
                services.TryGetValue(ServiceName, out service);
                if (service != null)
                {
                    return service.Execute(ServiceParam);
                }
            }
            catch { }
            return false;
        }
    }
}

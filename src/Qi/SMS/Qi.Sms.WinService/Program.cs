using System;
using System.IO;
using System.ServiceProcess;
using log4net.Config;

namespace Qi.Sms.WinService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(file);
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
                                {
                                    new QiSmsService()
                                };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
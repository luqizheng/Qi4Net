using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net.Config;

[assembly: XmlConfigurator(Watch = true)]

namespace Qi.Sms.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new QiSmsService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}

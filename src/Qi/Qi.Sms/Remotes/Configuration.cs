using System;
using System.Configuration;

namespace Qi.Sms.Remotes
{
    public static class Configuration
    {
        public static string PortName
        {
            get { return ConfigurationManager.AppSettings["SMS_PORT_Name"] ?? "COM1"; }
        }

        public static int BaudRate
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["SMS_BaudRate"] ?? "38400"); }
        }

        public static ISmsHandler SmsHandler
        {
            get
            {
                if (ConfigurationManager.AppSettings["SMS_Handler"] != null)
                {
                    object s = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["SMS_Handler"]));
                    var result = s as ISmsHandler;
                    return result;
                }
                return null;
            }
        }
    }
}
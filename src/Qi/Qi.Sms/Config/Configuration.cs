using System;
using System.Configuration;
using Qi.Sms.Remotes;

namespace Qi.Sms.Config
{
    public static class Configuration
    {
        public static string PortName
        {
            get { return ConfigurationManager.AppSettings["SMS_PORT_NAME"] ?? "COM1"; }
        }

        public static int BaudRate
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["SMS_BAUD_RATE"] ?? "38400"); }
        }

        public static ISmsHandler SmsHandler
        {
            get
            {
                if (ConfigurationManager.AppSettings["SMS_Handler"] != null)
                {
                    object s =
                        Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["SMS_Handler"]));
                    var result = s as ISmsHandler;
                    return result;
                }
                return null;
            }
        }

        #region Nested type: Remoteing

        public static class Remoteing
        {
            public static int Port
            {
                get { return Convert.ToInt32(ConfigurationManager.AppSettings["SMS_SERVICE_PORT"] ?? "12568"); }
            }

            public static string ServiceName
            {
                get { return ConfigurationManager.AppSettings["SMS_SERVICE_NAME"] ?? "QI_SERVICE_NAME"; }
            }
        }

        #endregion
    }
}
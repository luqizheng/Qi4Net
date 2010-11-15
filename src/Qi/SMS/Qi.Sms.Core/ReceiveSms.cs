using System;
using log4net.Config;

[assembly: XmlConfigurator(Watch = true)]

namespace Qi.Sms
{
    public class ReceiveSms
    {
        public string SendMobile { get; set; }
        public string Content { get; set; }
        public DateTime ReceiveTime { get; set; }
    }
}
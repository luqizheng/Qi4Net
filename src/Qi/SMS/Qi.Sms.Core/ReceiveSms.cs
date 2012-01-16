using System;
using log4net.Config;



namespace Qi.Sms
{
    public class ReceiveSms
    {
        public string SendMobile { get; set; }
        public string Content { get; set; }
        public DateTime ReceiveTime { get; set; }
    }
}
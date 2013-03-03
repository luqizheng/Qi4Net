using System;

namespace Qi.Sms.Protocol.Encodes
{
    public class ReceiveSms
    {
        public DateTime ReceiveTime { get; set; }
        public string SendMobile { get; set; }
        public string MessageCenterNumber { get; set; }
        public string Content { get; set; }
    }
}
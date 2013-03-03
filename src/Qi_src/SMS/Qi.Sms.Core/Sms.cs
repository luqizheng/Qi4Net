using System;

namespace Qi.Sms
{
    public class Sms
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
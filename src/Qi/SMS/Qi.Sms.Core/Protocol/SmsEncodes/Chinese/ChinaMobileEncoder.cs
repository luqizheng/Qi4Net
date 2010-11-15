using System;

namespace Qi.Sms.Protocol.SmsEncodes.Chinese
{
    internal class ChinaMobileEncoder : ChineseSms
    {
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Decode(string receiveName, out ReceiveSms sms)
        {
            throw new NotImplementedException();
        }

        protected override string PduEncode(string phone, string smsContent)
        {
            throw new NotImplementedException();
        }
    }
}
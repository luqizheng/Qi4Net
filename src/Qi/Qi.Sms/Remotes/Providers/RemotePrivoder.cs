using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Sms.Remotes.Providers
{
    
    public class RemotePrivoder : MarshalByRefObject, ISmsProvider
    {
        public static SmsProvider SmsProvider;
        #region IDisposable Members

        public void Dispose()
        {
            SmsProvider.Dispose();
        }

        #endregion

        #region ISmsProvider Members

        public void Send(string mobile, string content, Qi.Sms.Protocol.SendCommands.SmsFormat type)
        {
            SmsProvider.Send(mobile, content, type);
        }

        public ReceiveSms GetSms(int index)
        {
            return SmsProvider.GetSms(index);
        }

        public void Delete(int smsIndex)
        {
            SmsProvider.Delete(smsIndex);
        }

        #endregion
    }
}

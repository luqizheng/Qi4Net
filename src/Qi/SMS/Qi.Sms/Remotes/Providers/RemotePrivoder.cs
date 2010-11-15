using System;
using log4net;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes.Providers
{
    public class RemotePrivoder : MarshalByRefObject, ISmsProvider
    {
        private ILog log;

        public RemotePrivoder()
        {
            log = LogManager.GetLogger(this.GetType());
        }


        #region ISmsProvider Members

        public void Send(string mobile, string content, SmsFormat type)
        {
            try
            {
                SmsProvider.Instance.Send(mobile, content, type);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public ReceiveSms GetSms(int index)
        {
            try
            {
                return SmsProvider.Instance.GetSms(index);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }

        }

        public void Delete(int smsIndex)
        {
            try
            {
                SmsProvider.Instance.Delete(smsIndex);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        #endregion

    }
}
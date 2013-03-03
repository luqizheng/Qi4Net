using System;
using log4net;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes.Providers
{
    public class RemotePrivoder : MarshalByRefObject, ISmsProvider, IDisposable
    {
        private ILog log;
        private SmsProvider _vider;
        public RemotePrivoder()
        {
            log = LogManager.GetLogger(this.GetType());
            _vider = new SmsProvider();
        }


        #region ISmsProvider Members

        public void Send(string mobile, string content, SmsFormat type)
        {
            try
            {
                _vider.Send(mobile, content, type);
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
                return _vider.GetSms(index);
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
                _vider.Delete(smsIndex);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        #endregion

        public void Dispose()
        {
            _vider.Dispose();
            log.Info("disposing the Privoder.");
        }
    }
}
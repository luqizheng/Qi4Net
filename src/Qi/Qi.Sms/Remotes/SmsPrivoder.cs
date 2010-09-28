using System;
using System.Threading;
using log4net;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes
{
    public class SmsProvider : ISmsProvider
    {
        public static readonly SmsProvider Instance = new SmsProvider();
        private readonly SmsService _service;
        private readonly ISmsHandler handler;
        private ILog log;
        private SmsProvider()
        {
            var com = new ComConnection(Configuration.PortName, Configuration.BaudRate);
            handler = Configuration.SmsHandler;
            _service = new SmsService(com);
            _service.ReceiveSmsEvent += _service_NewSmsEvent;
            log = LogManager.GetLogger(this.GetType());
        }

        #region ISmsProvider Members

        public void Send(string mobile, string content, SmsFormat type)
        {
            bool result = true;
            if (handler != null)
            {
                log.InfoFormat("send a new message, content:{0}, target mobile:{1},SmsFromat:{2}", content, mobile,type.ToString());
                result = handler.OnSending(mobile, content, type);
            }
            if (result)
            {
                ThreadPool.QueueUserWorkItem(state => ((SmsService)state).Send(mobile, content, type), _service);
            }
        }

        #endregion

        public ReceiveSms GetSms(int index)
        {
            return _service.GetSms(index);
        }

        public void Delete(int smsIndex)
        {
            log.InfoFormat("Delete sms, index is smsIndex.");
            ThreadPool.QueueUserWorkItem(state => ((SmsService)state).Delete(smsIndex), _service);
        }

        private void _service_NewSmsEvent(object sender, NewMessageEventHandlerArgs e)
        {
            try
            {
                int index = e.SmsIndex;
                ReceiveSms sms = _service.GetSms(index);
                log.InfoFormat("Receive new sms,content:{0},Mobile{1}", sms.Content, sms.SendMobile);
                if (handler != null)
                {
                    if (!handler.OnReceived(sms))
                        Delete(e.SmsIndex);
                }
            }
            catch (Exception ex)
            {
                log.Error("Get a new message ,but found the error.", ex);
                Delete(e.SmsIndex);
                throw;
            }
        }
    }
}
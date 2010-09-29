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
        private readonly ISmsHandler _handler;
        private readonly SmsService _service;
        private readonly ILog log;

        private SmsProvider()
        {
            var com = new ComConnection(Configuration.PortName, Configuration.BaudRate);
            _handler = Configuration.SmsHandler;
            _service = new SmsService(com);
            _service.ReceiveSmsEvent += ServiceNewSmsEvent;
            log = LogManager.GetLogger(GetType());
        }

        #region ISmsProvider Members

        public void Send(string mobile, string content, SmsFormat type)
        {
            bool sendSms = true;
            if (_handler != null)
            {
                log.InfoFormat("send a new message, content:{0}, target mobile:{1},SmsFromat:{2}", content, mobile, type);
                sendSms = _handler.OnSending(mobile, content, type);
            }
            if (sendSms)
            {
                ThreadPool.QueueUserWorkItem(state => ((SmsService) state).Send(mobile, content, type), _service);
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
            ThreadPool.QueueUserWorkItem(state => ((SmsService) state).Delete(smsIndex), _service);
        }

        private void ServiceNewSmsEvent(object sender, NewMessageEventHandlerArgs e)
        {
            try
            {
                int index = e.SmsIndex;
                ReceiveSms sms = _service.GetSms(index);
                log.InfoFormat("Receive new sms,content:{0},Mobile{1}", sms.Content, sms.SendMobile);
                if (_handler != null)
                {
                    if (!_handler.OnReceived(sms))
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
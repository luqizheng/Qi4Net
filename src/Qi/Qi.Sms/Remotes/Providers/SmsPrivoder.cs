using System;
using System.Threading;
using log4net;
using Qi.Sms.Config;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes.Providers
{
    public class SmsProvider : MarshalByRefObject, ISmsProvider
    {
        public static readonly SmsProvider Instance = new SmsProvider();
        private readonly ISmsHandler _handler;
        private readonly SmsService _service;
        private readonly ILog _log;

        public SmsProvider()
        {
            var com = new ComConnection(Configuration.PortName, Configuration.BaudRate);
            _handler = Configuration.SmsHandler;
            _service = new SmsService(com);
            _service.ReceiveSmsEvent += ServiceNewSmsEvent;
            _log = LogManager.GetLogger(GetType());
        }

        #region ISmsProvider Members

        public void Send(string mobile, string content, SmsFormat type)
        {
            try
            {
                bool sendSms = true;
                if (_handler != null)
                {
                    _log.InfoFormat("send a new message, content:{0}, target mobile:{1},SmsFromat:{2}", content, mobile,
                                    type);
                    sendSms = _handler.OnSending(mobile, content, type);
                }
                if (sendSms)
                {
                    ThreadPool.QueueUserWorkItem(state => ((SmsService)state).Send(mobile, content, type), _service);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Send sms fail.", ex);
            }
        }

        public ReceiveSms GetSms(int index)
        {
            return _service.GetSms(index);
        }

        public void Delete(int smsIndex)
        {
            _log.InfoFormat("Delete sms, index is smsIndex.");
            ThreadPool.QueueUserWorkItem(state => ((SmsService)state).Delete(smsIndex), _service);
        }

        #endregion

        private void ServiceNewSmsEvent(object sender, NewMessageEventHandlerArgs e)
        {
            try
            {
                int index = e.SmsIndex;
                ReceiveSms sms = _service.GetSms(index);
                _log.InfoFormat("Receive new sms,content:{0},Mobile{1}", sms.Content, sms.SendMobile);
                if (_handler != null)
                {
                    if (!_handler.OnReceived(sms))
                        Delete(e.SmsIndex);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Get a new message ,but found the error.", ex);
                Delete(e.SmsIndex);
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Qi.Sms.Protocol;
using Qi.Sms.Protocol.Encodes;
using Qi.Sms.Protocol.SendCommands;
using log4net;
using log4net.Config;



namespace Qi.Sms
{
    public class SmsService : IDisposable
    {
        private const int maxSmsLength = 70;
        private readonly IDeviceConnection _deviceConnectin;
        private string _serviceCenterNumber;
        private object lockItem = "";
        private ILog _log;
        public SmsService(IDeviceConnection connection, string mobile)
            : this(connection)
        {
            _serviceCenterNumber = mobile;
            _log = LogManager.GetLogger(this.GetType());
        }

        public SmsService(IDeviceConnection connection)
        {
            _log = LogManager.GetLogger(this.GetType());
            ChinaMobile = true;
            if (connection == null)
                throw new ArgumentNullException("connection");
            _log.Debug("connect is ok.");
            _deviceConnectin = connection;
            _deviceConnectin.ReceivedEvent += DeviceConnectinReceivedEvent;
        }

        public string ServiceCenterNumber
        {
            get
            {
                if (_serviceCenterNumber == null)
                    _serviceCenterNumber = GetServicePhone();
                return _serviceCenterNumber;
            }
            set { _serviceCenterNumber = value; }
        }

        public bool ChinaMobile { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            _deviceConnectin.Dispose();
        }

        #endregion

        /// <summary>
        /// Not finish
        /// </summary>
        public void SetSmsAutoRecieve()
        {
            MakeSureConnection();
            _deviceConnectin.Send(new CNMICommand
                                      {
                                          NotifyMode = NotifyMode.Cache,
                                          SaveSaveMode = CnmiSaveMode.MemoryOnly
                                      });
        }


        private void DeviceConnectinReceivedEvent(object sender, DeviceCommandEventHandlerArgs e)
        {
            //var cmd = new CmtiCommand();
            foreach (var index in e.SmsIndex)
            {
                OnNewMessageEventHandlerArgs(index);
            }

        }

        public event EventHandler<CommandEventHandlerArgs> SendingEvent;
        public event EventHandler<CommandEventHandlerArgs> ReceivedEvent;
        public event EventHandler<NewMessageEventHandlerArgs> ReceiveSmsEvent;

        public static string[] CutMessageFromContent(string message)
        {
            var result = new List<string>();
            int current = 0;

            while (current < message.Length)
            {
                int end = current + maxSmsLength > message.Length ? message.Length - current : maxSmsLength;
                string newMessage = message.Substring(current, end);
                result.Add(newMessage);
                current += end;
            }
            return result.ToArray();
        }

        public bool Send(string phone, string sms, SmsFormat format)
        {
            lock (lockItem)
            {
                MakeSureConnection();
                sms = sms.Replace("\r\n", "");

                string[] contentSet = CutMessageFromContent(sms);
                if (ChinaMobile && contentSet.Length > 1)
                {
                    return SendMobile(phone, sms, format);
                }
                for (int i = 0; i < contentSet.Length; i++)
                {
                    string content = contentSet[i];
                    var messageFromat = new SetSmsFromatCommand(format);
                    AbstractCommand resultCommand = Send(messageFromat);
                    if (!resultCommand.Success)
                        return false;

                    var cmgsCommand = new CmgsCommand();

                    if (format == SmsFormat.Pdu)
                    {
                        var info = new SmsInfo(ServiceCenterNumber, phone, content);
                        int smsLen = 0;
                        content = info.EncodingSMS(out smsLen);
                        cmgsCommand.Argument = string.Format(smsLen.ToString("D2"));
                    }
                    else
                    {
                        cmgsCommand.Argument = phone;
                    }

                    resultCommand = Send(cmgsCommand);
                    if (!resultCommand.Success)
                        return false;
                    var directCommand = new SendContent
                                            {
                                                Content = content
                                            };
                    resultCommand = Send(directCommand);
                    if (!resultCommand.Success)
                        return false;

                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
            return true;
        }

        private bool SendMobile(string phone, string sms, SmsFormat format)
        {
            int rem = 0;
            int count = Math.DivRem(sms.Length, maxSmsLength, out rem);
            if (rem > 0)
            {
                count++;
            }
            var messageFromat = new SetSmsFromatCommand(format);
            AbstractCommand resultCommand = Send(messageFromat);
            this._log.Debug("Set format command is " + resultCommand.Success);
            if (!resultCommand.Success)
            {

                return false;
            }
            sms = PDUdecoding.EncodingOther(sms);
            this._log.Debug("send long sms's length " + count);
            for (int i = 0; i < count; i++)
            {
                _log.Info("Send the " + i);
                string length;
                string sendMessage = PDUdecoding.EncodingSMS(ServiceCenterNumber, phone, count, i + 1, sms, out length);
                var cmgs = new CmgsCommand { Argument = length };
                Send(cmgs);
                var directCommand = new SendContent
                                        {
                                            Content = sendMessage
                                        };
                var s = Send(directCommand);


            }
            return true;
        }

        private void MakeSureConnection()
        {
            if (!_deviceConnectin.IsConnected)
            {
                _deviceConnectin.Open();
            }
        }

        private AbstractCommand Send(AbstractCommand command)
        {
            if (SendingEvent != null)
                SendingEvent(this, new CommandEventHandlerArgs(command));

            string value = _deviceConnectin.Send(command);
            //var result = (AbstractCommand)command.Clone();
            //result.Init(value);
            //OnReceivedEvent(result);
            return command;
            //return result;
        }

        public ReceiveSms GetSms(int position)
        {
            lock (lockItem)
            {
                var cmglCommand = (CmgrCommand)Send(new CmgrCommand { MessageIndex = position });

                return new ReceiveSms
                           {
                               Content = cmglCommand.Content,
                               SendMobile = cmglCommand.SendMobile,
                               ReceiveTime = cmglCommand.ReceiveTime
                           };
            }
        }

        private void OnReceivedEvent(AbstractCommand comm)
        {
            if (ReceivedEvent != null)
            {
                ReceivedEvent(this, new CommandEventHandlerArgs(comm));
            }
        }

        private void OnNewMessageEventHandlerArgs(int smsIndex)
        {
            if (ReceiveSmsEvent != null)
            {
                ReceiveSmsEvent(this, new NewMessageEventHandlerArgs(smsIndex));
            }
        }

        public string GetServicePhone()
        {
            MakeSureConnection();
            var result = (CscaCommand)Send(new CscaCommand());
            return result.ServiceCenterNumber;
        }

        public bool Delete(int i)
        {
            return Send(new DeleteSms { SmsIndex = i }).Success;
        }
    }
}
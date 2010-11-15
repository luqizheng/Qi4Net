using System;
using System.Collections.Generic;
using System.Threading;
using log4net.Config;
using Qi.Sms.Protocol;
using Qi.Sms.Protocol.Encodes;
using Qi.Sms.Protocol.SendCommands;

[assembly: XmlConfigurator(Watch = true)]

namespace Qi.Sms
{
    public class SmsService : IDisposable
    {
        private const int maxSmsLength = 70;
        private readonly IDeviceConnection _deviceConnectin;
        private string _serviceCenterNumber;

        public SmsService(IDeviceConnection connection, string mobile)
            : this(connection)
        {
            _serviceCenterNumber = mobile;
        }

        public SmsService(IDeviceConnection connection)
        {
            ChinaMobile = true;
            if (connection == null)
                throw new ArgumentNullException("connection");

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
            var cmd = new CmtiCommand();
            if (cmd.Init(e.Command))
            {
                OnNewMessageEventHandlerArgs(cmd);
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
            lock (typeof (SmsService))
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
            if (!resultCommand.Success)
                return false;
            sms = PDUdecoding.EncodingOther(sms);
            for (int i = 0; i < count; i++)
            {
                string length;

                string sendMessage = PDUdecoding.EncodingSMS(ServiceCenterNumber, phone, count, i + 1, sms, out length);
                var cmgs = new CmgsCommand {Argument = length};
                resultCommand = Send(cmgs);
                if (!resultCommand.Success)
                    return false;
                var directCommand = new SendContent
                                        {
                                            Content = sendMessage
                                        };
                resultCommand = Send(directCommand);
                if (!resultCommand.Success)
                    return false;

                Thread.Sleep(1000);
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
            var result = (AbstractCommand) command.Clone();
            result.Init(value);
            OnReceivedEvent(result);
            return result;
        }

        public ReceiveSms GetSms(int position)
        {
            var cmglCommand = (CmgrCommand) Send(new CmgrCommand {MessageIndex = position});
            return new ReceiveSms
                       {
                           Content = cmglCommand.Content,
                           SendMobile = cmglCommand.SendMobile,
                           ReceiveTime = cmglCommand.ReceiveTime
                       };
        }

        private void OnReceivedEvent(AbstractCommand comm)
        {
            if (ReceivedEvent != null)
            {
                ReceivedEvent(this, new CommandEventHandlerArgs(comm));
            }
        }

        private void OnNewMessageEventHandlerArgs(CmtiCommand comm)
        {
            if (ReceiveSmsEvent != null)
            {
                ReceiveSmsEvent(this, new NewMessageEventHandlerArgs(comm));
            }
        }

        public string GetServicePhone()
        {
            MakeSureConnection();
            var result = (CscaCommand) Send(new CscaCommand());
            return result.ServiceCenterNumber;
        }

        public bool Delete(int i)
        {
            return Send(new DeleteSms {SmsIndex = i}).Success;
        }
    }
}
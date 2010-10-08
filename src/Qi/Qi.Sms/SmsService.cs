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
        private readonly IDeviceConnection _deviceConnectin;


        public SmsService(IDeviceConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _deviceConnectin = connection;
            _deviceConnectin.ReceivedEvent += DeviceConnectinReceivedEvent;
        }

        public string ServiceCenterNumber { get; set; }

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
            const int maxSmsLength = 70;
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
            lock (typeof(SmsService))
            {
                MakeSureConnection();
                sms = sms.Replace("\r\n", "");

                string[] contentSet = CutMessageFromContent(sms);
                for (int i = 0; i < contentSet.Length; i++)
                {
                    string content = contentSet[i];
                    var messageFromat = new SetSmsFromatCommand(format);
                    var resultCommand = Send(messageFromat);
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
                                                Content = string.Format("{0}{1}", content, (char)26)
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
            var result = (AbstractCommand)command.Clone();
            result.Init(value);
            OnReceivedEvent(result);
            return result;
        }

        public ReceiveSms GetSms(int position)
        {
            var cmglCommand = (CmgrCommand)Send(new CmgrCommand { MessageIndex = position });
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
            var result = (CscaCommand)Send(new CscaCommand());
            return result.ServiceCenterNumber;
        }

        public bool Delete(int i)
        {
            return Send(new DeleteSms { SmsIndex = i }).Success;
        }
    }
}
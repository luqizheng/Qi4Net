using System;
using System.Collections.Generic;
using System.Threading;
using Qi.Sms.Protocol.Encodes;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public class SmsService : IDisposable
    {
        private readonly IDeviceConnection _deviceConnectin;


        public SmsService(IDeviceConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _deviceConnectin = connection;
            _deviceConnectin.ReceivedEvent += _deviceConnectin_ReceivedEvent;
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
            _deviceConnectin.Send("AT+CNMI=2,1\r\n", true);
        }


        private void _deviceConnectin_ReceivedEvent(object sender, DeviceCommandEventHandlerArgs e)
        {
            var cmd = new CMTICommand();
            if (cmd.Init(e.Command))
            {
                OnNewMessageEventHandlerArgs(cmd);
            }
        }

        public event EventHandler<CommandEventHandlerArgs> SendingEvent;
        public event EventHandler<CommandEventHandlerArgs> ReceivedEvent;
        public event EventHandler<NewMessageEventHandlerArgs> NewSmsEvent;

        public static string[] CutMessageFromContent(string message)
        {
            var result = new List<string>();
            int current = 0;
            const int MaxSMSLength = 70;
            while (current < message.Length)
            {
                int end = current + MaxSMSLength > message.Length ? message.Length - current : MaxSMSLength;
                string newMessage = message.Substring(current, end);
                result.Add(newMessage);
                current += end;
            }
            return result.ToArray();
        }

        public void Send(string phone, string sms, SmsFormat format)
        {
            MakeSureConnection();
            sms = sms.Replace("\r\n", "");
            string[] contentSet = CutMessageFromContent(sms);
            for (int i = 0; i < contentSet.Length; i++)
            {
                string content = contentSet[i];
                var messageFromat = new SetSmsFromatCommand(format);
                Send(messageFromat);

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

                Send(cmgsCommand);
                var directCommand = new DirectCommand(string.Format("{0}{1}", content, (char) 26));
                Send(directCommand);
                Thread.Sleep(500);
            }
        }

        private void MakeSureConnection()
        {
            if (!_deviceConnectin.IsConnected)
            {
                _deviceConnectin.Open();
            }
        }

        private BaseCommand Send(BaseCommand command)
        {
            if (SendingEvent != null)
                SendingEvent(this, new CommandEventHandlerArgs(command));

            string value = _deviceConnectin.Send(command);
            var result = (BaseCommand) command.Clone();
            result.Init(value);
            OnReceivedEvent(result);
            return result;
        }

        public Sms GetSms(int position)
        {
            var cmglCommand = (CMGRCommand) Send(new CMGRCommand {MessageIndex = position});
            return new Sms
                       {
                           Content = cmglCommand.Content,
                           SendMobile = cmglCommand.SendMobile,
                           ReceiveTime = cmglCommand.ReceiveTime
                       };
        }

        private void OnReceivedEvent(BaseCommand comm)
        {
            if (ReceivedEvent != null)
            {
                ReceivedEvent(this, new CommandEventHandlerArgs(comm));
            }
        }

        private void OnNewMessageEventHandlerArgs(CMTICommand comm)
        {
            if (NewSmsEvent != null)
            {
                NewSmsEvent(this, new NewMessageEventHandlerArgs(comm));
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
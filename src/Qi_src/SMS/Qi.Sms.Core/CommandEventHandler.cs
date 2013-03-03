using System;
using Qi.Sms.Protocol;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms
{
    public class DeviceCommandEventHandlerArgs : EventArgs
    {
        public DeviceCommandEventHandlerArgs(int[] receiverSmsIndex)
        {
            SmsIndex = receiverSmsIndex;
        }

        public int[] SmsIndex { get; private set; }
    }

    public class CommandEventHandlerArgs : EventArgs
    {
        public CommandEventHandlerArgs(AbstractCommand command)
        {
            Command = command;
        }

        public AbstractCommand Command { get; private set; }
        public bool Cancel
        {
            get;
            set;
        }
    }

    public class NewMessageEventHandlerArgs : EventArgs
    {
        public NewMessageEventHandlerArgs(int smsIndex)
        {
            SmsIndex = smsIndex;
        }

        public int SmsIndex { get; private set; }
    }
}
using System;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public class DeviceCommandEventHandlerArgs : EventArgs
    {
        public DeviceCommandEventHandlerArgs(string command)
        {
            Command = command;
        }

        public string Command { get; private set; }
    }

    public class CommandEventHandlerArgs : EventArgs
    {
        public CommandEventHandlerArgs(BaseCommand command)
        {
            Command = command;
        }

        public BaseCommand Command { get; private set; }
    }

    public class NewMessageEventHandlerArgs : EventArgs
    {
        public NewMessageEventHandlerArgs(CMTICommand commad)
        {
            this.SmsIndex = commad.SmsIndex;
        }

        public int SmsIndex
        {
            get; private set;
        }
    }
}
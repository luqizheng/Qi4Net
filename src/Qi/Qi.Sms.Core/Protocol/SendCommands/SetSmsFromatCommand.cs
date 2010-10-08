using System;

namespace Qi.Sms.Protocol.SendCommands
{
    public enum SmsFormat
    {
        Pdu = 0,
        Text = 1,
    }

    public class SetSmsFromatCommand : AtCommand
    {
        public SetSmsFromatCommand()
        {
        }

        public SetSmsFromatCommand(SmsFormat smsFormat)
        {
            Format = smsFormat;
        }

        public override string Command
        {
            get { return "CMGF"; }
        }


        public SmsFormat Format
        {
            get { return (SmsFormat)Enum.ToObject(typeof(SmsFormat), Convert.ToInt32(Arguments[0])); }
            set
            {
                string number = Convert.ToInt32(value).ToString();
                if (Arguments.Count == 0)
                    Arguments.Add(number);
                else
                    Arguments[0] = number;
            }
        }

        protected override bool InitContent(string content)
        {
            return true;
        }
    }
}
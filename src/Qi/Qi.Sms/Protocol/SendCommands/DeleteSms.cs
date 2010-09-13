using System;

namespace Qi.Sms.Protocol.SendCommands
{
    public class DeleteSms : BaseCommand
    {
        public override string Command
        {
            get { return "CMGD"; }
        }

        protected override bool InitContent(string content)
        {
            if (!content.Contains(Command))
                return false;
            return true;
        }
        public int SmsIndex
        {
            get
            {
                if (this.Arguments.Count == 0)
                    return -1;
                return Convert.ToInt32(this.Arguments[0]);
            }
            set
            {
                if (this.Arguments.Count == 0)
                    this.Arguments.Add(value.ToString());
                else
                {
                    this.Arguments[0] = value.ToString();
                }
            }
        }
    }
}

using System;
using System.Text.RegularExpressions;

namespace Qi.Sms.Protocol.SendCommands
{
    public class CMTICommand : BaseCommand
    {
        public override string Command
        {
            get { return "CMTI"; }
        }

        public int SmsIndex { get; set; }

        protected override bool InitContent(string content)
        {
            //+CMTI: "SM",1
            bool result = content.Contains("+CMTI:");
            if (result)
            {

                SmsIndex = Convert.ToInt32(Regex.Replace(content, "\\D", ""));

            }
            return result;
        }
    }
}
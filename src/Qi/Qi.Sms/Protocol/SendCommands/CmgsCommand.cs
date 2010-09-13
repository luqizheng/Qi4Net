using Qi.Sms.Protocol;

namespace Qi.Sms.Protocol.SendCommands
{
    public class CmgsCommand : BaseCommand
    {
        public CmgsCommand()
        {
            //this.NoReturnValue = true;
            Arguments.Add("");
        }

        public override string Command
        {
            get { return "CMGS"; }
        }

        public string Argument
        {
            get { return Arguments[0]; }
            set { Arguments[0] = value; }
        }

        protected override bool InitContent(string content)
        {
            return content.Contains("AT+CMGS") && content.Contains(Argument) && content.Contains(">");
        }
    }
}
using System.Text.RegularExpressions;

namespace Qi.Sms.Protocol.SendCommands
{
    public class CscaCommand : AtCommand
    {
        public CscaCommand()
        {
            Arguments.Add("?");
        }

        public override string Command
        {
            get { return "CSCA"; }
        }

        public string ServiceCenterNumber { get; private set; }

        protected override bool InitContent(string content)
        {
            if (!content.Contains("+" + Command + ":"))
                return false;

            Match match = Regex.Match(content, "\".*\"");
            if (match.Success)
            {
                string resultValue = match.Value;
                ServiceCenterNumber = resultValue.Substring(1, resultValue.Length - 2);
            }
            return true;
        }
    }
}
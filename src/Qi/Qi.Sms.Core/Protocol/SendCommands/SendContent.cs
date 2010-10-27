namespace Qi.Sms.Protocol.SendCommands
{
    public class SendContent : AbstractCommand
    {
        public string Content { get; set; }

        public override string Command
        {
            get { return Content; }
        }

        protected override bool InitContent(string content)
        {
            return true;
            //return content.Contains("+CMGS") && !content.Contains("AT+");
        }

        public override string CompleteCommand()
        {
            return string.Format("{0}\x01a", Content);;
        }
    }
}
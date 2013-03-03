namespace Qi.Sms.Protocol.SendCommands
{
    public class SendContent : AbstractCommand
    {
        public string Content { get; set; }

        public override string Command
        {
            get { return Content; }
        }

        public override string CompleteCommand()
        {
            return string.Format("{0}\x01a", Content);;
        }
    }
}
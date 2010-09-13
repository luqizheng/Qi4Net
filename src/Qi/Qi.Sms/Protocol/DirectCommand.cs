namespace Qi.Sms.Protocol
{
    public class DirectCommand : BaseCommand
    {
        private readonly string _command;

        public DirectCommand(string command)
        {
            this.NoReturnValue = true;
            _command = command;
        }

        public DirectCommand()
        {
        }


        public override string Command
        {
            get { return _command; }
        }


        protected override bool InitContent(string content)
        {
            return true;
        }

        public override string ToString()
        {
            return Command + "\r\n";
        }
    }
}
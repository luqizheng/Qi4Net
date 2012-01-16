using System.Threading;

namespace Qi.Sms.Protocol
{
    public class DirectCommand : AbstractCommand
    {
        private readonly string _command;

        public DirectCommand()
        {
            TimeSleep = 500;
        }

        public DirectCommand(string command)
        {
            _command = command;
            TimeSleep = 500;
        }

        public int TimeSleep { get; set; }


        public override string Command
        {
            get { return _command; }
        }

    

        public override string CompleteCommand()
        {
            Thread.Sleep(TimeSleep);
            return Command;
        }
    }
}
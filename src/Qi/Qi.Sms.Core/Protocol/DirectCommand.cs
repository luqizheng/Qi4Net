using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qi.Sms.Protocol
{
    public class DirectCommand : AbstractCommand
    {
        string _command;
        public DirectCommand(string command)
        {
            this._command = command;
            this.TimeSleep = 500;
        }

        public int TimeSleep
        {
            get;
            set;
        }


        public override string Command
        {
            get { return _command; }
        }

        protected override bool InitContent(string content)
        {
            return true;
        }

        public override string CompleteCommand()
        {
            Thread.Sleep(this.TimeSleep);
            return Command;
        }
    }
}

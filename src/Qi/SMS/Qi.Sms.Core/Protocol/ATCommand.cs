using System;

namespace Qi.Sms.Protocol
{
    public abstract class AtCommand : AbstractCommand
    {
        public override string CompleteCommand()
        {
            string arguments;
            if (Arguments.Count == 0)
            {
                arguments = "";
            }
            else
            {
                if (Arguments[0] != "?")
                    arguments = "=" + String.Join(",", Arguments.ToArray());
                else
                    arguments = "?";
            }

            return string.Format("AT+{0}{1}{2}", Command, arguments, CompleteSymblo);
        }

        protected virtual string CompleteSymblo
        {
            get { return "\r\n"; }
        }

    }
}
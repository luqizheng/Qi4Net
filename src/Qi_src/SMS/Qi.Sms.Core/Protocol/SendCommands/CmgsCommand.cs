using System;

namespace Qi.Sms.Protocol.SendCommands
{
    public class CmgsCommand : AtCommand
    {
        public CmgsCommand()
        {
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

        public override bool Init(string command)
        {
            var a = command.Contains(">");
            Success = a;
            return a;
        }

     
    }
}
using System;
using System.Collections.Generic;
using log4net;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public abstract class BaseCommand : ICloneable
    {
        private static readonly IList<BaseCommand> CommandSet = new List<BaseCommand>();

        private List<string> _arguments;
        protected ILog log;

        static BaseCommand()
        {
            CommandSet.Add(new CmgsCommand());
            CommandSet.Add(new SetSmsFromatCommand());
            CommandSet.Add(new CscaCommand());
        }

        protected BaseCommand()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public abstract string Command { get; }

        protected List<string> Arguments
        {
            get { return _arguments ?? (_arguments = new List<string>()); }
        }

        /// <summary>
        /// Command is ok?
        /// </summary>
        public bool Success { get; set; }

        public bool NoReturnValue { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            var result = (BaseCommand)Activator.CreateInstance(GetType());
            result.Success = Success;
            foreach (string item in Arguments)
            {
                result.Arguments.Add(item);
            }
            return result;
        }

        #endregion

        public virtual bool Init(string command)
        {
            Success = command.Contains("OK");
            return InitContent(command);
        }

        protected abstract bool InitContent(string content);
        
        public string CompleteCommand()
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

            return string.Format("AT+{0}{1}", Command, arguments);
        }
        public override string ToString()
        {
            return CompleteCommand();
        }

        public static IList<BaseCommand> CreateCommand(string command)
        {
            var result = new List<BaseCommand>();
            foreach (BaseCommand baseCommand in CommandSet)
            {
                if (command.Contains(baseCommand.Command))
                {
                    //有可能返回2条记录。
                    var receiveCommand = (BaseCommand)baseCommand.Clone();
                    receiveCommand.Init(command);
                    result.Add(receiveCommand);
                }
            }
            return result;
        }
    }
}
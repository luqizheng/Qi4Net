using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public abstract class AbstractCommand: ICloneable
    {
        private static readonly IList<AbstractCommand> CommandSet = new List<AbstractCommand>();

        private List<string> _arguments;
        protected ILog log;

        static AbstractCommand()
        {
            CommandSet.Add(new CmgsCommand());
            CommandSet.Add(new SetSmsFromatCommand());
            CommandSet.Add(new CscaCommand());
        }

        protected AbstractCommand()
        {
            log = LogManager.GetLogger(GetType());
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
            var result = (AbstractCommand)Activator.CreateInstance(GetType());
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

        public abstract string CompleteCommand();
        
        public override string ToString()
        {
            return CompleteCommand();
        }

        public static IList<AbstractCommand> CreateCommand(string command)
        {
            var result = new List<AbstractCommand>();
            foreach (var baseCommand in CommandSet)
            {
                if (command.Contains(baseCommand.Command))
                {
                    //有可能返回2条记录。
                    var receiveCommand = (AbstractCommand)baseCommand.Clone();
                    receiveCommand.Init(command);
                    result.Add(receiveCommand);
                }
            }
            return result;
        }
    }
}

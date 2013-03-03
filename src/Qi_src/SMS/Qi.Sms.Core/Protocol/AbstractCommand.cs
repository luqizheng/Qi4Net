using System;
using System.Collections.Generic;
using Qi.Sms.Protocol.SendCommands;
using log4net;

namespace Qi.Sms.Protocol
{
    public abstract class AbstractCommand : ICloneable
    {
        private static readonly IList<AbstractCommand> CommandSet = new List<AbstractCommand>();

        protected ILog Log;
        private List<string> _arguments;

        /// <summary>
        /// 
        /// </summary>
        static AbstractCommand()
        {
            CommandSet.Add(new CmgsCommand());
            CommandSet.Add(new SetSmsFromatCommand());
            CommandSet.Add(new CscaCommand());
        }

        /// <summary>
        /// 
        /// </summary>
        protected AbstractCommand()
        {
            Log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract string Command { get; }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> Arguments
        {
            get { return _arguments ?? (_arguments = new List<string>()); }
        }

        /// <summary>
        /// Command is ok?
        /// </summary>
        public bool Success { get; set; }

        #region ICloneable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var result = (AbstractCommand) Activator.CreateInstance(GetType());
            result.Success = Success;
            foreach (string item in Arguments)
            {
                result.Arguments.Add(item);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Init command from string command, if this command belong to this command return true, or return false.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual bool Init(string command)
        {
            bool result = false;
            if (command.ToUpper().Contains("OK"))
            {
                Success = true;
                result = true;
            }
            else if (command.ToUpper().Contains("ERROR"))
            {
                Success = false;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string CompleteCommand();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CompleteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static IList<AbstractCommand> CreateCommand(string command)
        {
            var result = new List<AbstractCommand>();
            foreach (AbstractCommand baseCommand in CommandSet)
            {
                if (command.Contains(baseCommand.Command))
                {
                    //有可能返回2条记录。
                    var receiveCommand = (AbstractCommand) baseCommand.Clone();
                    receiveCommand.Init(command);
                    result.Add(receiveCommand);
                }
            }
            return result;
        }
    }
}
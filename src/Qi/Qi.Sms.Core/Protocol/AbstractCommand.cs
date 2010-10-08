using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public abstract class AbstractCommand : ICloneable
    {
        private static readonly IList<AbstractCommand> CommandSet = new List<AbstractCommand>();

        private List<string> _arguments;

        protected ILog Log;
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



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var result = (AbstractCommand)Activator.CreateInstance(GetType());
            result.Success = Success;
            foreach (var item in Arguments)
            {
                result.Arguments.Add(item);
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual bool Init(string command)
        {
            if (command.ToUpper().Contains("OK"))
            {
                Success = true;
                return InitContent(command);
            }
            else if (command.ToUpper().Contains("ERROR"))
            {
                Success = false;
                return InitContent(command);
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected abstract bool InitContent(string content);
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

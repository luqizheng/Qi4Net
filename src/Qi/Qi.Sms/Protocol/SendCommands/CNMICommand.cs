using System;

namespace Qi.Sms.Protocol.SendCommands
{
    public class CNMICommand : AtCommand
    {
        /// <summary>
        /// defautl set to 2,1
        /// </summary>
        public CNMICommand()
        {
            SaveSaveMode = CnmiSaveMode.MemoryOnly;
            NotifyMode = NotifyMode.Cache;
        }

        public override string Command
        {
            get { return "CNMI"; }
        }

        public CnmiSaveMode SaveSaveMode
        {
            get
            {
                int v = Convert.ToInt32(Arguments[1]);
                return (CnmiSaveMode) Enum.ToObject(typeof (CnmiSaveMode), v);
            }
            set { Arguments[1] = Convert.ToInt32(value).ToString(); }
        }

        public NotifyMode NotifyMode
        {
            get
            {
                int v = Convert.ToInt32(Arguments[0]);
                return (NotifyMode) Enum.ToObject(typeof (NotifyMode), v);
            }
            set { Arguments[0] = Convert.ToInt32(value).ToString(); }
        }

        protected override bool InitContent(string content)
        {
            return content.Contains("CNMI");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CnmiSaveMode
    {
        /// <summary>
        /// 0 – 储存到默认的内存位置(包括class 3) value is 0
        /// </summary>
        SaveToDefaultMemory = 0,
        /// <summary>
        ///  储存到默认的内存位置，并且向TE发出通知(包括class 3)
        /// </summary>
        MemoryOnly = 1,
        /// <summary>
        /// 对于class 2，储存到SIM卡，并且向TE发出通知；对于其它class，直接将消息转发到 TE
        /// </summary>
        SimOnly = 2,
        /// <summary>
        /// 对于class 3，直接将消息转发到 TE；对于其它class，同mt=1
        /// </summary>
        SendTo = 3
    }

    public enum NotifyMode
    {
        /// <summary>
        /// 不通知TE。 
        /// </summary>
        None = 0,
        /// <summary>
        /// 只在数据线空闲的情况下，通知TE；否则不通知TE。 
        /// </summary>
        OnlyInSpearTime = 1,
        /// <summary>
        /// 通知TE。在数据线被占用的情况下，先缓冲起来，待数据线空闲，再行通知。 
        /// </summary>
        Cache = 2,
        /// <summary>
        /// 通知TE。在数据线被占用的情况下，通知混合在数据中一起传输。 
        /// </summary>
        Mix = 3,
    }
}
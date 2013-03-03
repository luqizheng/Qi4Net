using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.NHibernate
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NhConfigurationException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public NhConfigurationException(string msg)
            : base(msg)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerexception"></param>
        public NhConfigurationException(string msg, Exception innerexception)
            : base(msg, innerexception)
        {

        }
    }
}

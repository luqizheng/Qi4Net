using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Nhibernates
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SessionManagerException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SessionManagerException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactoryKey"></param>
        /// <param name="message"></param>
        public SessionManagerException(string sessionFactoryKey, string message)
            : base(message + ",session factory key is " + sessionFactoryKey)
        {

        }
    }
}

using System;

namespace Qi.Net

{
    /// <summary>
    /// 
    /// </summary>
    public class NetworkException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public NetworkException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
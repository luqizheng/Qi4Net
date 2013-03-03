using System;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// </summary>
    public class NHModelBinderException : ApplicationException
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public NHModelBinderException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="formatException"></param>
        public NHModelBinderException(string message, Exception formatException)
            : base(message, formatException)
        {
        }
    }
}
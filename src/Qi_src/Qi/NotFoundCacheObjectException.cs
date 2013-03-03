using System;

namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    public class NotFoundCacheObjectException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public NotFoundCacheObjectException(int key)
            : base("Can't find the object by key=" + key)
        {
        }
    }
}
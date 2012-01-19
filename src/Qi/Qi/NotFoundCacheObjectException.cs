using System;

namespace Qi
{
    public class NotFoundCacheObjectException : ApplicationException
    {
        public NotFoundCacheObjectException(int key)
            : base("Can't find the object by key=" + key)
        {
        }
    }
}
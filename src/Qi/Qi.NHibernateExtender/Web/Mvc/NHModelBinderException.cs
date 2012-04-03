using System;

namespace Qi.Web.Mvc
{
    public class NHModelBinderException : ApplicationException
    {
        public NHModelBinderException(string message):base(message)
        {
            
        }
        public NHModelBinderException(string message,Exception formatException)
            : base(message,formatException)
        {
        }
    }
}
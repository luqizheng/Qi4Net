using System;

namespace Qi.Net

{
    public class NetworkException : ApplicationException
    {
        public NetworkException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
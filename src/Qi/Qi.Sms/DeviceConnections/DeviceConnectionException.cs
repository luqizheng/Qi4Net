using System;

namespace Qi.Sms.DeviceConnections
{
    public class DeviceConnectionException : ApplicationException
    {
        public DeviceConnectionException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
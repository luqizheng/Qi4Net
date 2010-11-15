using System;

namespace Qi.Sms.DeviceConnections
{
    [Serializable]
    public class DeviceConnectionException : ApplicationException
    {
        public DeviceConnectionException()
        {
            
        }
        
        public DeviceConnectionException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
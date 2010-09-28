using System;
using Qi.Sms.Protocol;

namespace Qi.Sms
{
    public interface IDeviceConnection : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler<DeviceCommandEventHandlerArgs> SendingEvent;
        event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;
        void Open();
        void Close();
        string Send(AbstractCommand command);
    }
}
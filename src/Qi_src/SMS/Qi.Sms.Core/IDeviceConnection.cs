using System;
using System.IO.Ports;
using Qi.Sms.Protocol;

namespace Qi.Sms
{
    public interface IDeviceConnection : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;
        void Open();
        void Close();
        string Send(AbstractCommand command);
        void InvokeSend(AbstractCommand command, int sleepMilSeconds);
        SerialPort SerialPort { get; }
    }
}
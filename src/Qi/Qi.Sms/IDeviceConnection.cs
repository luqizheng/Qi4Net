using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]  
namespace Qi.Sms.Protocol
{
    public interface IDeviceConnection : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler<DeviceCommandEventHandlerArgs> SendingEvent;
        event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;
        void Open();
        void Close();
        string Send(BaseCommand command);
        string Send(string command, bool noReturnValue);
    }
}
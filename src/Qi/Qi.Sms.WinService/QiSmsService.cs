using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceProcess;
using System.Threading;
using log4net;
using Qi.Sms.Config;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Remotes.Providers;

namespace Qi.Sms.WinService
{
    public partial class QiSmsService : ServiceBase
    {
        private readonly ILog _log;
        private IChannel _smsChannel;
        

        public QiSmsService()
        {
            InitializeComponent();
            _log = LogManager.GetLogger(GetType());
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _log.Info("Start Sms Service");
                _smsChannel = new TcpChannel(Configuration.Remoteing.Port);
                ChannelServices.RegisterChannel(_smsChannel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof (RemotePrivoder),
                                                                   Configuration.Remoteing.ServiceName,
                                                                   WellKnownObjectMode.Singleton);
            }
            catch (DeviceConnectionException ex)
            {
                _log.Error("Start sms service have error", ex);
            }
            catch (Exception ex)
            {
                _log.Error("Start sms service have error", ex);
                Thread.Sleep(1000);
            }
        }

        protected override void OnStop()
        {
            try
            {
                ChannelServices.UnregisterChannel(_smsChannel);
            }
            catch (Exception ex)
            {
                _log.Error("Sms Service Stop error.", ex);
            }
        }
    }
}
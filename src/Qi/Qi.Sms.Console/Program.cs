using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using Qi.Sms.Config;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Protocol.SendCommands;
using Qi.Sms.Remotes.Providers;

namespace Qi.Sms.ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            try
            {
               // _log.Info("Start Sms Service");

                var _smsChannel = new TcpChannel(Configuration.Remoteing.Port);
                ChannelServices.RegisterChannel(_smsChannel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(SmsProvider),
                                                                   Configuration.Remoteing.ServiceName,
                                                                   WellKnownObjectMode.Singleton);
            }
            catch (DeviceConnectionException ex)
            {
               // _log.Error("Start sms service have error", ex);
            }
            catch (Exception ex)
            {
               // _log.Error("Start sms service have error", ex);
                Thread.Sleep(1000);
            }

            //try
            //{
            //    var conn = new ComConnection("COM8", 9600);
            //    var service = new SmsService(conn);
            //    service.ServiceCenterNumber = service.GetServicePhone();
            //    ThreadPool.QueueUserWorkItem(MultiSend1, service);
            //    //ThreadPool.QueueUserWorkItem(MultiSend2, service);
            //    //ThreadPool.QueueUserWorkItem(MultiSend3, service);
            //}
            //catch (Exception ex)
            //{
            //    Exception exx = ex;
            //    Console.Read();
            //}
            Console.Read();
            Console.Read();
            Console.Read();
        }

        public static void MultiSend1(object state)
        {
            var sender = (SmsService) state;
            var sb = new StringBuilder();
            for (int i = 0; i < 141; i++)
            {
                sb.Append("一");
            }
            sender.Send("8613532290006", sb.ToString(), SmsFormat.Pdu);
        }

        public static void MultiSend2(object state)
        {
            var sender = (SmsService) state;
            var sb = new StringBuilder();
            for (int i = 0; i < 210; i++)
            {
                sb.Append("二");
            }
            sender.Send("13532290006", sb.ToString(), SmsFormat.Pdu);
        }

        public static void MultiSend3(object state)
        {
            var sender = (SmsService) state;
            var sb = new StringBuilder();
            for (int i = 0; i < 210; i++)
            {
                sb.Append("三");
            }
            sender.Send("13532290006", sb.ToString(), SmsFormat.Pdu);
        }
    }
}
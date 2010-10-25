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
            //Console.WriteLine(String.Format("\x01a"));
            //Console.ReadLine();
            //try
            //{
            //    // _log.Info("Start Sms Service");
            //    RemotePrivoder.SmsProvider=new SmsProvider();
            //    var _smsChannel = new TcpChannel(Configuration.Remoteing.Port);
            //    ChannelServices.RegisterChannel(_smsChannel, false);
            //    RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotePrivoder),
            //                                                       Configuration.Remoteing.ServiceName,
            //                                                       WellKnownObjectMode.Singleton);
            //}
            //catch (DeviceConnectionException ex)
            //{
            //    // _log.Error("Start sms service have error", ex);
            //}
            //catch (Exception ex)
            //{
            //    // _log.Error("Start sms service have error", ex);
            //    Thread.Sleep(1000);
            //}
            var conn = new ComConnection("COM3", 9600);
            conn.Open();
            try
            {


                //while (true)
                //{
                //    string command = Console.ReadLine();
                //    //var com = new Protocol.SendCommands.SetSmsFromatCommand(SmsFormat.Pdu);
                //    conn.Send(command);
                //}
                var service = new SmsService(conn, "8613800756500");
                //service.ChinaMobile = false;


                //service.ServiceCenterNumber = "8613800756500";//service.GetServicePhone();
                ThreadPool.QueueUserWorkItem(MultiSend1, service);
                //ThreadPool.QueueUserWorkItem(MultiSend2, service);
                //ThreadPool.QueueUserWorkItem(MultiSend3, service);
            }
            catch (Exception ex)
            {
                Exception exx = ex;
                Console.Read();
            }
            finally
            {
                conn.Close();
            }
            Console.Read();
            Console.Read();
            Console.Read();
        }

        public static void MultiSend1(object state)
        {
            var sender = (SmsService)state;
            var sb = new StringBuilder("其实这是一个中文长短信的测试，这里有多少字，我其实也不知道，不过只要超过70个字符就可以了，所以就随便写写东西上去，如一二三，你知道我不知道但他知道的事情。");

            sender.Send("8613532290006", sb.ToString(), SmsFormat.Pdu);
        }

        public static void MultiSend2(object state)
        {
            var sender = (SmsService)state;
            var sb = new StringBuilder();
            for (int i = 0; i < 210; i++)
            {
                sb.Append("二");
            }
            sender.Send("13532290006", sb.ToString(), SmsFormat.Pdu);
        }

        public static void MultiSend3(object state)
        {
            var sender = (SmsService)state;
            var sb = new StringBuilder();
            for (int i = 0; i < 210; i++)
            {
                sb.Append("三");
            }
            sender.Send("13532290006", sb.ToString(), SmsFormat.Pdu);
        }
    }
}
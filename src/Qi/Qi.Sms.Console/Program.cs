using System;
using System.Text;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var conn = new ComConnection("COM3", 38400);
                var service = new SmsService(conn);
                var sb = new StringBuilder();
                sb.Append("一".PadRight(70, '测'));
                sb.Append("二".PadRight(70, '测'));
                sb.Append("三".PadRight(70, '测'));

                string mscPhone = service.GetServicePhone();
                service.ServiceCenterNumber = mscPhone;
                Console.Read();
                service.Send("135xxxxxx", sb.ToString(), SmsFormat.Pdu);
                Console.Read();
                Console.Read();
                Console.Read();
            }
            catch (Exception ex)
            {
                Exception exx = ex;
                //Console.Read();
            }
        }
    }
}
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
                var conn = new ComConnection("COM1", 38400);
                var service = new SmsService(conn);                
            }
            catch (Exception ex)
            {
                Exception exx = ex;
                //Console.Read();
            }
        }
    }
}
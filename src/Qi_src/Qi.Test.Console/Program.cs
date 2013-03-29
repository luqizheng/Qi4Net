using Qi.Net;

namespace Qi.Test.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            /*var a = A.Apple;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");
            var key = a.ToDescription();
            System.Console.WriteLine(key);
            System.Console.ReadLine();*/
            PingHost ping = new PingHost();
            var replay = ping.Ping("www.chiphell.com");
            System.Console.WriteLine("{0},time;{1}ms", replay.Address, replay.RoundtripTime);

            /*var rote = new TraceRoute();
            rote.SuccessArrive += rote_SuccessArrive;
            rote.NextTry += rote_NextTry;
            rote.Start("www.126.com");*/
            System.Console.ReadLine();
        }

        private static void rote_NextTry(object sender, TraceRouteArgs e)
        {
            System.Console.WriteLine("TTlExpire arrite at {0},time:{1},ttl:{2}", e.Replay.Address, e.Replay.Status, e.Ttl);

        }

        private static void rote_SuccessArrive(object sender, TraceRouteArgs e)
        {
            System.Console.WriteLine("Success arrite at   {0},time:{1}", e.Replay.Address, e.Replay.Status);
        }

        private enum A
        {
            [EnumDescription("appleKey", ResourceType = typeof(Resource))]
            Apple
        }
    }
}
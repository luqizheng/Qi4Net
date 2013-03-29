using System;
using System.Net.NetworkInformation;
using System.Text;

namespace Qi.Net
{
    public class TraceRoute
    {
        private const string Data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        public TraceRoute()
        {
            MaxTtl = 30;
            MinTtl = 1;
            Timeout = 10000;
        }

        /// <summary>
        ///     Timeout milliseconds.
        /// </summary>
        public int Timeout { get; set; }

        public int MinTtl { get; set; }
        public int MaxTtl { get; set; }

        public event EventHandler<TraceRouteArgs> SuccessArrive;
        public event EventHandler<TraceRouteArgs> NextTry;

        public void Start(string hostnameOraddress)
        {
            for (int i = MinTtl; i < MaxTtl; i++)
            {
                if (!GetTraceRoute(hostnameOraddress, i))
                {
                    break;
                }
            }
        }

        private bool GetTraceRoute(string hostNameOrAddress, int ttl)
        {
            var pinger = new Ping();
            var pingerOptions = new PingOptions(ttl, true);

            byte[] buffer = Encoding.ASCII.GetBytes(Data);
            PingReply reply = default(PingReply);

            reply = pinger.Send(hostNameOrAddress, Timeout, buffer, pingerOptions);


            if (reply.Status == IPStatus.Success)
            {
                if (SuccessArrive != null)
                    SuccessArrive(this, new TraceRouteArgs(reply, ttl));
                return false;
            }
            /*else if (reply.Status == IPStatus.TtlExpired)
            {*/
            if (NextTry != null)
                NextTry(this, new TraceRouteArgs(reply, ttl));
            return true;
            /*}
            return true;*/
        }
    }
}
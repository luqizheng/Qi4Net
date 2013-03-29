using System;
using System.Net.NetworkInformation;

namespace Qi.Net
{
    public class TraceRouteArgs : EventArgs
    {
        public TraceRouteArgs(PingReply replay, int ttl)
        {
            Replay = replay;
            Ttl = ttl;
        }

        public PingReply Replay { get; private set; }
        public int Ttl { get; set; }
    }
}
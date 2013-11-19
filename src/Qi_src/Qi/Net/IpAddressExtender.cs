using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Qi.Net
{
    public static class IpAddressExtender
    {
        /// <summary>
        /// Check ip is in min to max;
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool In(this IPAddress ip, IPAddress min, IPAddress max)
        {
            var lip = ip.ToInt64();
            var lmin = min.ToInt64();
            var lMax = min.ToInt64();
            return lip <= lMax && lip >= lmin;
        }

        public static long ToInt64(this IPAddress ip)
        {
            int x = 3;

            long o = 0;

            foreach (byte f in ip.GetAddressBytes())
            {

                o += (long)f << 8 * x--;

            }

            return o;
        }
    }
}
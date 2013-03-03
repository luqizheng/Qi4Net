using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Qi.Net
{
    public static class IpAddressExtender
    {
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        /// <summary>
        /// 通过IPAddress 获取Mac地址，只是用于同网段
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="NetworkException">failed to get client mac</exception>
        public static string GetMac(this IPAddress client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            try
            {
                int ldest = inet_addr(client.ToString()); //目的地的ip

                var macinfo = new Int64();
                int len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string macSrc = macinfo.ToString("X");
                string macDest = "";

                while (macSrc.Length < 12)
                {
                    macSrc = macSrc.Insert(0, "0");
                }
                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i%2))
                    {
                        macDest = i == 10
                                      ? macDest.Insert(0, macSrc.Substring(i, 2))
                                      : "-" + macDest.Insert(0, macSrc.Substring(i, 2));
                    }
                }
                return macDest;
            }
            catch (Exception ex)
            {
                throw new NetworkException(String.Format("Can't find mac from {0}", client), ex);
            }
        }
    }
}
using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace Qi.Web
{
    public static class WebExtender
    {
        public static IPAddress GetClientIp(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            return GetClientIp(request.ServerVariables);
        }

        public static IPAddress GetClientIp(NameValueCollection serverVariables)
        {
            string ip = serverVariables["HTTP_VIA"] != null
                         ? serverVariables["HTTP_X_FORWARDED_FOR"]
                         : serverVariables["REMOTE_ADDR"];
            return IPAddress.Parse(ip);
        }
    }
}
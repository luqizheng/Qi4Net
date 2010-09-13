using System;
using System.Net;
using System.Web;

namespace Qi.Web
{
    public static class WebExtender
    {
        public static IPAddress GetClientIp(this HttpRequest request)
        {
            if(request==null)
                throw new ArgumentNullException("request");
            var ip = request.ServerVariables["HTTP_VIA"] != null
                            ? request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                            : request.ServerVariables["REMOTE_ADDR"];
            return IPAddress.Parse(ip);
        }
    }
}
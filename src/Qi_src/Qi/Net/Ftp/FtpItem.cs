using System;
using System.IO;
using System.Net;

namespace Qi.Net.Ftp
{
    internal delegate void ResponseHandler(StreamReader reader);
    public class FtpItem
    {
        public FtpItem(string uri)
        {
            this.Uri = new Uri(uri);
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Gets the Uri of this Ftp link.
        /// </summary>
        protected Uri Uri { get; set; }


        /// <summary>
        /// Create FtpWebRequest from current <see cref="Uri"/>
        /// </summary>
        /// <returns></returns>
        protected virtual FtpWebRequest Create()
        {
            var result = (FtpWebRequest)WebRequest.Create(Uri);
            result.UseBinary = true;
            if (!String.IsNullOrEmpty(UserName))
            {
                result.Credentials = new NetworkCredential(UserName, Password);
            }
            return result;
        }

        //protected  static FtpItem CreateItemBy(string directoryDetialLine)
        //{

        //}
    }
}
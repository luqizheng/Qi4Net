using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Qi.Net.Ftp
{
    public class FtpDirectory : FtpItem
    {
        private FtpDirectory(string ip, int port)
            : base(String.Format("ftp://{0}:{1}", ip, port))
        {
        }

        public FtpDirectory(string uri)
            : base(uri)
        {
        }

        public static FtpDirectory Connect(string ip, int port, string user, string password)
        {
            var result = new FtpDirectory(ip, port) { UserName = user, Password = password };

            return result;
        }

        public static FtpDirectory Connect(string uri, string user, string password)
        {
            var result = new FtpDirectory(uri) { UserName = user, Password = password };

            return result;
        }

        public IList<FtpFile> GetFiles()
        {
            var result = new List<FtpFile>();
            ListDirectoryDetial(reader =>
                                    {
                                        string line = reader.ReadLine();
                                        while (line != null)
                                        {
                                            if (!line.StartsWith("f"))
                                            {
                                                line = reader.ReadLine();
                                                continue;
                                            }
                                            result.Add(new FtpFile(Uri + "/" + line.Substring(50), UserName, Password));
                                            line = reader.ReadLine();
                                        }
                                    });

            return result;
        }

        public IList<FtpDirectory> GetDirectoies()
        {
            var result = new List<FtpDirectory>();
            ListDirectoryDetial(reader =>
                                    {
                                        string line = reader.ReadLine();
                                        while (line != null)
                                        {
                                            if (!line.StartsWith("d"))
                                            {
                                                line = reader.ReadLine();
                                                continue;
                                            }
                                            result.Add(new FtpDirectory(Uri + "/" + line.Substring(50))
                                                           {
                                                               UserName = UserName,
                                                               Password = Password
                                                           });
                                            line = reader.ReadLine();
                                        }
                                    });
            return result;
        }

        public void Upload(Stream stream)
        {
        }

        private void ListDirectoryDetial(ResponseHandler method)
        {
            FtpWebRequest request = Create();
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                try
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            method(reader);
                        }
                    }
                }
                finally
                {
                    response.Close();
                }
            }
        }
    }
}
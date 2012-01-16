using System.IO;
using System.Net;

namespace Qi.Net.Ftp
{
    public class FtpFile : FtpItem
    {
        public FtpFile(string uri, string userName, string password)
            : base(uri)
        {
            UserName = userName;
            Password = password;
        }

        public Stream Download()
        {
            FtpWebRequest result = Create();
            result.Method = WebRequestMethods.Ftp.DownloadFile;
            return result.GetRequestStream();
        }

        public void Download(string savePath)
        {
            using (Stream s = Download())
            {
                using (FileStream fileStream = File.Create(savePath))
                {
                    var buffer = new byte[1024];
                    while (true)
                    {
                        int bytesRead = s.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}
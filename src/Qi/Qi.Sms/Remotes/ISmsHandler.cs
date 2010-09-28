using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes
{
    public interface ISmsHandler
    {
        /// <summary>
        /// 收到短信的时候，返回true表示保留短信，返回false表示删除短信。
        /// </summary>
        /// <param name="receiveSms"></param>
        /// <returns></returns>
        bool OnReceived(ReceiveSms receiveSms);
        /// <summary>
        /// 当发送Sms的之前,返回true，表示可以发送，返回false，表示不发送
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        bool OnSending(string mobile, string content, SmsFormat format);
    }
}
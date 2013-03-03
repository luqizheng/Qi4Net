using log4net;
using Qi.Sms.Protocol.SendCommands;
using Qi.Sms.Remotes;

namespace Qi.Sms.ConsoleTest
{
    public class TestSmsHandler : ISmsHandler
    {
        private readonly ILog log = LogManager.GetLogger(typeof (TestSmsHandler));

        #region ISmsHandler Members

        public ISmsProvider Priovider { get; set; }

        public bool OnReceived(ReceiveSms receiveSms)
        {
            log.InfoFormat("{0} {1},{2}", receiveSms.SendMobile, receiveSms.Content, receiveSms.ReceiveTime);
            return false;
        }

        public bool OnSending(string mobile, string content, SmsFormat format)
        {
            return true;
        }

        #endregion
    }
}
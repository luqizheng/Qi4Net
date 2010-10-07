using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol
{
    public interface IEncoder
    {
        /// <summary>
        /// Gets the name of thie encoder
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or Sets the sms servier's number
        /// </summary>
        string SmsCenterNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        string Encode(string phone, string smsContent);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveName"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        bool Decode(string receiveName, out ReceiveSms sms);

        SmsFormat SmsFormat { get; set; }
    }
}
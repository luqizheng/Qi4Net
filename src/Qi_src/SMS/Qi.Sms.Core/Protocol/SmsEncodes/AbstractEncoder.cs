using System;
using System.Text.RegularExpressions;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Protocol.SmsEncodes
{
    public abstract class AbstractEncoder : IEncoder
    {
        #region IEncoder Members

        public abstract string Name { get; }

        public string SmsCenterNumber { get; set; }

        public string Encode(string phone, string smsContent)
        {
            if (phone == null)
                throw new ArgumentNullException("phone");
            if (smsContent == null)
                throw new ArgumentNullException("smsContent");
            phone = FormatPhone(phone);

            return PduEncode(phone, smsContent);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="receiveName"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        public abstract bool Decode(string receiveName, out ReceiveSms sms);

        /// <summary>
        /// Gets or sets SmsFormat
        /// </summary>
        public SmsFormat SmsFormat { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        protected abstract string PduEncode(string phone, string smsContent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        protected abstract string FormatPhone(string phone);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        protected bool OverMaxLength(string smsContent)
        {
            const int maxCharCount = 70; //最长可发送汉字个数
            int s = smsContent.Length/maxCharCount;
            return s > 1.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string GetLengthForPdu(string str)
        {
            int divRem;
            int rem = Math.DivRem(str.Length, 2, out divRem);
            if (divRem != 0)
                rem = rem + 1;
            return rem.ToString("X2");
        }

        /// <summary>
        /// 奇偶互换并补F
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ParityChange(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            if (!Regex.Match(value, "\\d*").Success)
            {
                throw new FormatException("Please input number.");
            }

            string result = string.Empty;
            int length = value.Length;
            for (int i = 1; i < length; i += 2) //奇偶互换
            {
                result += value[i];
                result += value[i - 1];
            }

            if (length%2 != 0) //不是偶数则加上F，与最后一位互换
            {
                result += 'F';
                result += value[length - 1];
            }
            return result;
        }
    }
}
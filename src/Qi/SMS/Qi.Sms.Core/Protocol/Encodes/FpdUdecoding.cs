using System;

namespace Qi.Sms.Protocol.Encodes
{
    internal static class FpdUdecoding
    {
        ///   <summary>     
        ///   判断接受的短信是PDU格式还是TEXT格式     
        ///   </summary>     
        public static bool IsPdu(string sms)
        {
            if (sms.Substring(40, 2) != "08")
                return false;
            return true;
        }

        ///   <summary>     
        ///   函数功能：短信内容提取     
        ///   函数名称：GetEverySMS(string   SMS)     
        ///   参         数：SMS   要进行提取的整个短信内容     
        ///   返   回   值：将多个短信内容拆分     
        ///   </summary>     
        public static string[] GetEverySms(string sms)
        {
            char[] str = "\n".ToCharArray();
            string[] temp = sms.Split(str);
            return temp;
        }

        ///   <summary>     
        ///   函数功能：提取短信的发送人电话号码     
        ///   函数名称：GetTelphone(string   SMS)     
        ///   参         数：SMS   要进行转换的整个短信内容     
        ///   返   回   值：电话号码     
        ///   </summary>     
        public static string GetTelphone(string sms)
        {
            string tel = sms.Substring(26, 12);
            string s = "";
            for (int i = 0; i < 9; i += 2)
            {
                s += tel[i + 1];
                s += tel[i];
            }
            s += tel[tel.Length - 1];
            return s;
        }

        ///   <summary>     
        ///   函数功能：提取短信的发送时间     
        ///   函数名称：GetDataTime(string   SMS)     
        ///   参         数：SMS:要进行转换的整个短信内容     
        ///   返   回   值：发送时间     
        ///   </summary>     
        public static DateTime GetDataTime(string sms)
        {
            string time = sms.Substring(42, 12);
            string s = "";
            for (int i = 0; i < 11; i += 2)
            {
                s += time[i + 1];
                s += time[i];
            }
            string t = s.Substring(0, 2) + "-" + s.Substring(2, 2) + "-" + s.Substring(4, 2) + "-" + s.Substring(6, 2) +
                       ":" + s.Substring(8, 2) + ":" + s.Substring(10, 2);
            return DateTime.Parse(t);
        }

        ///   <summary>     
        ///   函数功能：提取短信的内容(PDU)     
        ///   函数名称：GetContent(string   SMS)     
        ///   参         数：SMS:要进行转换的整个短信内容     
        ///   返   回   值：短信内容     
        ///   </summary>     
        public static string GetContent(string sms)
        {
            string c = "";
            string len = sms.Substring(56, 2);
            int length = Convert.ToInt16(len, 16);
            length *= 2;
            string content = sms.Substring(58, length);
            for (int i = 0; i < length; i += 4)
            {
                string temp = content.Substring(i, 4);
                int by = Convert.ToInt16(temp, 16);
                var ascii = (char) by;
                c += ascii.ToString();
            }
            return c;
        }

        ///   <summary>     
        ///   函数功能：提取短信的TEXT内容(TEXT)     
        ///   函数名称：GetTextContent(string   SMS)     
        ///   参         数：SMS:要进行转换的整个短信内容     
        ///   返   回   值：短信内容     
        ///   </summary>     
        public static string GetTextContent(string sms)
        {
            string str = "";
            string c = "";
            byte by;
            char ascii;
            int i;
            sms = sms.Replace("\r", "");
            sms = sms.Replace("\n", "");
            string content = sms.Substring(58);
            for (i = content.Length - 2; i >= 0; i -= 2)
            {
                by = Convert.ToByte(content.Substring(i, 2), 16);
                str += Convert.ToString(by, 2).PadLeft(8, '0');
            }
            for (i = str.Length - 7; i >= 0; i -= 7)
            {
                by = Convert.ToByte(str.Substring(i, 7), 2);
                ascii = (char) by;
                c += ascii.ToString();
            }
            return c;
        }
    }
}
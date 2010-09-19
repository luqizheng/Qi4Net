//以下代码来源于
//http://blog.csdn.net/duwx/archive/2009/03/07/3967017.aspx, 感谢klude。

using System;
using System.Text;

namespace Qi.Sms.Protocol.Encodes
{
    public static class PduEncoding
    {
        /// <summary>  
        /// 函数功能：短信内容编码  
        /// 函数名称：smsPDUEncoded(string srvContent)  
        /// 参    数：srvContent 要进行转换的短信内容,string类型  
        /// 返 回 值：编码后的短信内容，string类型  
        /// 程 序 员：klude  
        /// 编制日期：2007-11-04  
        /// 函数说明：  
        ///          1，采用Big-Endian 字节顺序的 Unicode 格式编码，也就说把高低位的互换在这里完成了  
        ///          2，将转换后的短信内容存进字节数组  
        ///          3，去掉在进行Unicode格式编码中，两个字节中的"-",例如：00-21，变成0021  
        ///          4，将整条短信内容的长度除2，保留两位16进制数  
        /// </summary>  
        private static string SmsPduEncoded(string srvContent)
        {
            Encoding encodingUtf = Encoding.BigEndianUnicode;
            string s = null;
            byte[] encodedBytes = encodingUtf.GetBytes(srvContent);
            for (int i = 0; i < encodedBytes.Length; i++)
            {
                s += BitConverter.ToString(encodedBytes, i, 1);
            }
            s = String.Format("{0:X2}{1}", s.Length/2, s);

            return s;
        }

        /// <summary>  
        /// 函数功能：短信中心号编码  
        /// 函数名称：smsDecodedCenterNumber(string srvCenterNumber)  
        /// 参    数：srvCenterNumber 要进行转换的短信中心号,string类型  
        /// 返 回 值：编码后的短信中心号，string类型  
        /// 程 序 员：klude  
        /// 编制日期：2007-11-04  
        /// 函数说明：  
        ///          1，将奇数位和偶数位交换。  
        ///          2，短信中心号奇偶数交换后，看看长度是否为偶数，如果不是，最后添加F  
        ///          3，加上短信中心号类型，91为国际化  
        ///          4，计算编码后的短信中心号长度，并格化成二位的十六进制  
        /// </summary>  
        public static string SmsDecodedCenterNumber(string srvCenterNumber)
        {
            string s = null;
            if (srvCenterNumber.Substring(0, 2) != "86")
            {
                srvCenterNumber = String.Format("86{0}", srvCenterNumber); //检查当前短信中心号是否按标准格式书写，不是，就补上“86”  
            }
            int length = srvCenterNumber.Length;
            for (int i = 1; i < length; i += 2) //奇偶互换  
            {
                s += srvCenterNumber[i];
                s += srvCenterNumber[i - 1];
            }
            if (length%2 != 0) //是否为偶数，不是就加上F，并对最后一位与加上的F位互换  
            {
                s += 'F';
                s += srvCenterNumber[length - 1];
            }
            s = String.Format("91{0}", s); //加上91,代表短信中心类型为国际化  
            s = String.Format("{0:X2}{1}", s.Length/2, s); //编码后短信中心号长度，并格式化成二位十六制  
            return s;
        }

        /// <summary>  
        /// 函数功能：接收短信手机号编码  
        /// 函数名称：smsDecodedNumber(string srvNumber)  
        /// 参    数：srvCenterNumber 要进行转换的短信中心号,string类型  
        /// 返 回 值：编码后的接收短信手机号，string类型  
        /// 程 序 员：klude  
        /// 编制日期：2007-11-04  
        /// 函数说明：  
        ///          1，检查当前接收手机号是否按标准格式书写，不是，就补上“86”  
        ///          1，将奇数位和偶数位交换。  
        ///          2，短信中心号奇偶数交换后，看看长度是否为偶数，如果不是，最后添加F  
        /// </summary>  
        public static string SmsDecodedNumber(string srvNumber)
        {
            string s = null;
            if (srvNumber.Substring(0, 2) != "86")
            {
                srvNumber = String.Format("86{0}", srvNumber); //检查当前接收手机号是否按标准格式书写，不是，就补上“86”  
            }
            int length = srvNumber.Length;
            for (int i = 1; i < length; i += 2) //将奇数位和偶数位交换  
            {
                s += srvNumber[i];
                s += srvNumber[i - 1];
            }
            if (length%2 != 0) //是否为偶数，不是就加上F，并对最后一位与加上的F位互换  
            {
                s += 'F';
                s += srvNumber[length - 1];
            }
            return s;
        }

        /// <summary>  
        /// 函数功能：整个短信的编码  
        /// 函数名称：smsDecodedsms(string strCenterNumber, string strNumber, string strSMScontent)  
        /// 参    数：strCenterNumber 要进行转换的短信中心号,string类型  
        ///           strNumber       接收手机号码，string类型  
        ///           strSMScontent   短信内容  
        /// 返 回 值：完整的短信编码，可以在AT指令中执行，string类型  
        /// 程 序 员：klude  
        /// 编制日期：2007-11-04  
        /// 函数说明：  
        ///           11000D91和000800   在国内，根据PDU编码原则，我们写死在此，详细解释请看我的文章      
        ///           31000D91//短信报告  
        /// </summary>  
        public static string SmsEncoding(string strCenterNumber, string strNumber, string strSmScontent,
                                         out string length)
        {
            string s = String.Format("{0}11000D91{1}000800{2}", SmsDecodedCenterNumber(strCenterNumber),
                                     SmsDecodedNumber(strNumber), SmsPduEncoded(strSmScontent));
            length = String.Format("{0:D2}", (s.Length - SmsDecodedCenterNumber(strCenterNumber).Length)/2);
            //获取短信内容加上手机号码长度  
            return s;
        }
    }

    public static class FpdUdecoding
    {
        ///   <summary>     
        ///   判断接受的短信是PDU格式还是TEXT格式     
        ///   </summary>     
        public static bool IsPdu(string SMS)
        {
            if (SMS.Substring(40, 2) != "08")
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
        public static string GetTelphone(string SMS)
        {
            string tel = SMS.Substring(26, 12);
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
        public static string GetDataTime(string SMS)
        {
            string time = SMS.Substring(42, 12);
            string s = "";
            for (int i = 0; i < 11; i += 2)
            {
                s += time[i + 1];
                s += time[i];
            }
            string t = s.Substring(0, 2) + "年" + s.Substring(2, 2) + "月" + s.Substring(4, 2) + "日" + s.Substring(6, 2) +
                       ":" + s.Substring(8, 2) + ":" + s.Substring(10, 2);
            return t;
        }

        ///   <summary>     
        ///   函数功能：提取短信的内容(PDU)     
        ///   函数名称：GetContent(string   SMS)     
        ///   参         数：SMS:要进行转换的整个短信内容     
        ///   返   回   值：短信内容     
        ///   </summary>     
        public static string GetContent(string SMS)
        {
            string c = "";
            string len = SMS.Substring(56, 2);
            int length = Convert.ToInt16(len, 16);
            length *= 2;
            string content = SMS.Substring(58, length);
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
        public static string GetTextContent(string SMS)
        {
            string str = "";
            string c = "";
            byte by;
            char ascii;
            int i;
            SMS = SMS.Replace("\r", "");
            SMS = SMS.Replace("\n", "");
            string content = SMS.Substring(58);
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
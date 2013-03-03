using System;
using System.Globalization;
using System.Text;

namespace Qi.Sms.Protocol.Encodes
{
    public static class CodingHelper
    {
        /// <summary>
        /// 对7-bit编码进行编码
        /// </summary>
        /// <param name="s">要编码的字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string EncodingBit7(string s)
        {
            int iLeft = 0;
            string sReturn = "";
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // 取源字符串的计数值的最低3位
                int iChar = i & 7;
                var bSrc = (byte) char.Parse(s.Substring(i, 1));
                // 处理源串的每个字节
                if (iChar == 0)
                {
                    // 组内第一个字节，只是保存起来，待处理下一个字节时使用
                    iLeft = char.Parse(s.Substring(i, 1));
                }
                else
                {
                    // 组内其它字节，将其右边部分与残余数据相加，得到一个目标编码字节
                    sReturn = (bSrc << (8 - iChar) | iLeft).ToString("X4");
                    // 将该字节剩下的左边部分，作为残余数据保存起来
                    iLeft = bSrc >> iChar;
                    // 修改目标串的指针和计数值 pDst++;
                    sb.Append(sReturn.Substring(2, 2));
                }
            }
            sb.Append(sReturn.Substring(0, 2));
            int udl = sb.Length/2;
            return udl.ToString("X2") + sb;
        }

        /// <summary>
        /// 对7-bit编码进行解码
        /// </summary>
        /// <param name="s">要解码的字符串</param>
        /// <returns>解码后的英文字符串</returns>
        public static string DecodingBit7(string s)
        {
            int iByte = 0;
            int iLeft = 0;
            // 将源数据每7个字节分为一组，解压缩成8个字节
            // 循环该处理过程，直至源数据被处理完
            // 如果分组不到7字节，也能正确处理
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i += 2)
            {
                byte bSrc = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
                // 将源字节右边部分与残余数据相加，去掉最高位，得到一个目标解码字节
                sb.Append((((bSrc << iByte) | iLeft) & 0x7f).ToString("X2"));
                // 将该字节剩下的左边部分，作为残余数据保存起来
                iLeft = bSrc >> (7 - iByte);
                // 修改字节计数值
                iByte++;
                // 到了一组的最后一个字节
                if (iByte == 7)
                {
                    // 额外得到一个目标解码字节
                    sb.Append(iLeft.ToString("X2"));
                    // 组内字节序号和残余数据初始化
                    iByte = 0;
                    iLeft = 0;
                }
            }
            string sReturn = sb.ToString();
            var buf = new byte[sReturn.Length/2];
            for (int i = 0; i < sReturn.Length; i += 2)
            {
                buf[i/2] = byte.Parse(sReturn.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// 使用8-bit进行编码
        /// </summary>
        /// <param name="s">要编码的字符串</param>
        /// <returns>信息长度及编码后的字符串</returns>
        public static string EncodingBit8(string s)
        {
            var sb = new StringBuilder();
            byte[] buf = Encoding.ASCII.GetBytes(s);
            sb.Append(buf.Length.ToString("X2"));
            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X2"));
            }

            int udl = sb.Length/2;
            return udl.ToString("X2") + sb;
        }

        /// <summary>
        /// 使用8-bit进行解码
        /// </summary>
        /// <param name="s">要解码的字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodingBit8(string s)
        {
            var buf = new byte[s.Length/2];
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i += 2)
            {
                buf[i/2] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// 中文短信息UCS2编码
        /// </summary>
        /// <param name="s">要编码的中文字符串</param>
        /// <returns>信息长度及编码后的字符串</returns>
        public static string EncodingUCS2(string s)
        {
            var sb = new StringBuilder();
            byte[] buf = Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < buf.Length; i += 2)
            {
                sb.Append(buf[i + 1].ToString("X2"));
                sb.Append(buf[i].ToString("X2"));
            }
            int udl = sb.Length/2;
            return udl.ToString("X2") + sb;
        }

        /// <summary>
        /// 中文短信息UCS2解码
        /// </summary>
        /// <param name="s">要解码的信息</param>
        /// <returns>解码后的中文字符串</returns>
        public static string DecodingUcs2(string s)
        {
            var buf = new byte[s.Length];
            for (int i = 0; i < s.Length; i += 4)
            {
                buf[i/2] = byte.Parse(s.Substring(2 + i, 2), NumberStyles.AllowHexSpecifier);
                buf[i/2 + 1] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.Unicode.GetString(buf);
        }

        /// <summary>
        /// 对电话号码进行编码
        /// </summary>
        /// <param name="s">电话号码,如："+8613010112500","8613010112500","13010112500"</param>
        /// <returns>编码后的电话号码如："913110102105F0",不包括长度</returns>
        public static string EncodingMobileNum(string s, string type, bool isSCSA)
        {
            s = s.Replace("+", "");
            if (s.StartsWith("86"))
            {
                s = s.Substring(2);
            }

            char[] charArray = (s + "F").ToCharArray();
            string addr = "";
            for (int i = 0; i < charArray.Length; i += 2)
            {
                char start = '0';
                if ((i + 1) <= charArray.Length)
                {
                    start = charArray[i + 1];
                }
                addr += start + charArray[i].ToString();
            }

            addr = type + "68" + addr;

            if (isSCSA)
            {
                int addrLen = addr.Length/2;
                addr = addrLen.ToString("X2") + addr;
            }
            else
            {
                int addrLen2 = s.Length + 2;
                addr = addrLen2.ToString("X2") + addr;
            }
            return addr;
        }

        /// <summary>
        /// 对电话号码进行解码
        /// </summary>
        /// <param name="s">要解码的电话号码,如:"3110102105F0"</param>
        /// <returns>解码后的电话号码如:13010112500</returns>
        public static string DecodingMobileNum(string s, string addrType)
        {
            if (s.Length%2 != 0)
            {
                s += "F";
            }
            char[] mobileNumArray = s.ToCharArray();
            string mobileNum = "";
            for (int i = 0; i < mobileNumArray.Length; i += 2)
            {
                mobileNum += mobileNumArray[i + 1] + mobileNumArray[i].ToString();
            }
            switch (addrType)
            {
                case "A1":
                    break;
                case "81":
                    break;
                case "91":
                    mobileNum = "+" + mobileNum;
                    break;
            }
            mobileNum = mobileNum.Replace("F", "");

            return mobileNum;
        }

        /// <summary>
        /// 对时间进行编码
        /// </summary>
        /// <param name="s">要编码的时间</param>
        /// <returns>编码后的时间</returns>
        public static string EncodingTime(DateTime t)
        {
            string time = t.ToString("yyMMddHHmmss");
            char[] timeChar = time.ToCharArray();
            string temp = "";
            for (int i = 0; i < timeChar.Length; i += 2)
            {
                temp += timeChar[i + 1] + timeChar[i].ToString();
            }
            return temp + "80"; //北京位于东八区，编码后为80
        }

        /// <summary>
        /// 对时间进行解码
        /// </summary>
        /// <param name="s">要解码的时间</param>
        /// <returns>解码后的时间</returns>
        public static DateTime DecodingTime(string s)
        {
            char[] timeChar = s.ToCharArray();
            string temp = "";
            for (int i = 0; i < timeChar.Length - 2; i += 2)
            {
                temp += timeChar[i + 1] + timeChar[i].ToString();
            }

            temp = temp.Substring(0, 2) + "-" + temp.Substring(2, 2) + "-" + temp.Substring(4, 2) + " " +
                   temp.Substring(6, 2) + ":" + temp.Substring(8, 2) + ":" + temp.Substring(10, 2);
            return DateTime.Parse(temp);
        }

        /// <summary>
        /// byte数组到字符串转换,采用ASCII编码格式
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string byteToText(byte[] byteArray)
        {
            string result = Encoding.Default.GetString(byteArray);
            return result;
        }
    }
}
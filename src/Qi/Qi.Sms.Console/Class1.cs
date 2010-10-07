using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Sms.ConsoleTest
{
    /// <summary>
    /// 针对国内短信编码（USC2）
    /// </summary>
    public class PDUdecoding
    {
        public readonly static int MAX_CHAR_COUNT = 70;//最长可发送汉字个数

        /// <summary>
        /// 奇偶互换并补F
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ParityChange(string value)
        {
            string result = string.Empty;
            int length = value.Length;
            for (int i = 1; i < length; i += 2)//奇偶互换
            {
                result += value[i];
                result += value[i - 1];
            }

            if (!(length % 2 == 0))//不是偶数则加上F，与最后一位互换
            {
                result += 'F';
                result += value[length - 1];
            }
            return result;
        }

        /// <summary>
        /// 短信内容编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// 采用Big-Endian 字节顺序的 Unicode 格式编码，将高低位互换
        /// 将转换后的短信内容存进字节数组
        /// 去掉在进行Unicode格式编码中，两个字节中的"-",例如：00-21，变成0021
        /// 将整条短信内容的长度除2，保留两位16进制数
        /// </remarks>
        public static string Encoding(string value)
        {
            Encoding encoding = System.Text.Encoding.BigEndianUnicode;
            string result = string.Empty;
            byte[] bytes = encoding.GetBytes(value);

            for (int i = 0; i < bytes.Length; i++)
            {
                result += BitConverter.ToString(bytes, i, 1);
            }

            return result;
        }

        public static string EncodeingAddLength(string value)
        {
            string result = Encoding(value);
            return String.Format("{0:X2}{1}", result.Length / 2, result);
        }

        /// <summary>
        /// 短信中心号码编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodingCenter(string phone)
        {
            string result = string.Empty;
            result = ParityChange(phone);

            result = String.Format("91{0}", result);//加上91
            result = String.Format("{0:X2}{1}", result.Length / 2, result);
            return result;
        }

        /// <summary>
        /// 接收短信手机号码编码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string DecodingPhone(string phone)
        {
            string result = string.Empty;

            if (null == phone || 0 == phone.Length)
            {
                return result;
            }

            if ('+' == phone[0])
            {
                phone = phone.TrimStart('+');
            }

            if (!(phone.Substring(0, 2) == "86"))//补86
            {
                phone = String.Format("86{0}", phone);
            }

            return ParityChange(phone);
        }

        /// <summary>
        /// 整个短信的编码
        /// </summary>
        /// <param name="center"></param>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <param name="length">要发送内容的长度,由两部分组成,接收手机号加上要发送的内容</param>
        /// <returns></returns>
        public static string EncodingSMS(string center, string phone, string content, out string length)
        {
            center = DecodingCenter(center);
            string result = String.Format("{0}11000D91{1}000800{2}", center, DecodingPhone(phone), EncodeingAddLength(content));
            length = String.Format("{0:D2}", result.Length / 2 - center.Length / 2);//获取短信内容加上手机号码长度
            return result;
        }

        /// <summary>
        /// 超长短信
        /// </summary>
        /// <param name="center"></param>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <param name="count"></param>
        /// <param name="i"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string EncodingSMS(string center, string phone, string content, int count, int i, out string length)
        {
            if (content.Length <= PDUdecoding.MAX_CHAR_COUNT)
            {
                return PDUdecoding.EncodingSMS(center, phone, content, out length);
            }
            else
            {
                if (count - 1 == i)
                {
                    content = content.Substring(i * (PDUdecoding.MAX_CHAR_COUNT - 6));
                }
                else
                {
                    content = content.Substring(i * (PDUdecoding.MAX_CHAR_COUNT - 6), PDUdecoding.MAX_CHAR_COUNT - 6);
                }

                center = DecodingCenter(center);
                content = Encoding(content);

                string result = "";

                DateTime tm = DateTime.Now;

                //result = string.Format("{0}44000D91{1}0008{2}{3:X2}05000304{4:D2}{5:D2}{6}",
                //                        center,
                //                        DecodingPhone(phone),
                //                        ParityChange(string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}08", tm.Year - 2000, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second)),
                //                        (content.Length + 12) / 2,
                //                        count,
                //                        i + 1,
                //                        content
                //                        );

                result = string.Format("005100{0}{1}0008A7{2:X2}05000304{3:D2}{4:D2}{5}",
                                        center,
                                        DecodingPhone(phone),
                                        (content.Length + 12) / 2,
                                        count,
                                        i + 1,
                                        content);

                length = String.Format("{0:D2}", result.Length / 2 - center.Length / 2);//获取短信内容加上手机号码长度
                return result;
            }
        }

        public static int GetMaxEncodeCharCount()
        {
            return PDUdecoding.MAX_CHAR_COUNT * 2 - 6;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="phone"></param>
        /// <param name="count"></param>
        /// <param name="i"></param>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string EncodingSMS(string center, string phone, int count, int i, string content, out string length)
        {
            string message = string.Empty;
            int msgcount = PDUdecoding.GetMaxEncodeCharCount() * 2;//固定

            if (i == count)
            {
                message = content.Substring((i - 1) * msgcount); //共可发送134个编码
            }
            else
            {
                message = content.Substring((i - 1) * msgcount, msgcount);
            }

            center = DecodingCenter(center);

            //string result = string.Format("005100{0}{1}0008A7{2:X2}05000304{3:D2}{4:D2}{5}",
            //                            center,
            //                            DecodingPhone(phone),
            //                            (message.Length + 12) / 2,
            //                            count,
            //                            i,
            //                            message);

            //length = String.Format("{0:D2}", result.Length / 2 - center.Length / 2);//获取短信内容加上手机号码长度

            ////string ph = phone;
            ////phone = DecodingPhone(phone).Substring(2);

            ////string result = string.Format("005100{0}{1}0008A7{2:X2}05000304{3:D2}{4:D2}{5}",
            ////                              string.Format("{0:X2}A1", ph.Length),
            ////                            phone,
            ////                            (message.Length + 12) / 2,
            ////                            count,
            ////                            i,
            ////                            message);

            string result = String.Format("{0}51000D91{1}000800{2:X2}05000304{4:D2}{5:D2}{3}",
                                            center,
                                            DecodingPhone(phone),
                                            message.Length / 2 + 6,
                                            message,
                                            count,
                                            i
                                            );

            length = String.Format("{0:D2}", result.Length / 2 - center.Length / 2);//获取短信内容加上手机号码长度

            return result;
        }

        public static string EncodingOther(string content)
        {
            string result = string.Empty;
            byte[] bytes = System.Text.Encoding.BigEndianUnicode.GetBytes(content);

            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    if (0x80 == (bytes[i] & 0x80))//汉字 6c49  108 73
            //    {
            //        result = string.Format("{0}{1:X2}", result, bytes[i]);
            //        result = string.Format("{0}{1:X2}", result, bytes[++i]);
            //    }
            //    else
            //    {
            //        result = string.Format("{0}00{1:X2}", result, bytes[i]);
            //    }
            //}
            foreach (char item in content)
            {
                bytes = System.Text.Encoding.BigEndianUnicode.GetBytes(new char[1] { item });
                if (bytes.Length < 2)
                    continue;

                //
                if (0x80 == (Asc(item) & 0x80))//汉字
                {
                    result = string.Format("{0}{1:X2}", result, bytes[0]);
                    result = string.Format("{0}{1:X2}", result, bytes[1]);
                }
                else
                {
                    result = string.Format("{0}00{1:X2}", result, bytes[1]);
                }
            }
            return result;
        }

        private static int Asc(char item)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(new char[1] { item });

            if (bytes.Length < 2)
                return bytes[0];

            return bytes[0] * 256 + bytes[1] - 65535;
        }
    }
}

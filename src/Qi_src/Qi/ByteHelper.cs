using System;
using System.Text;

namespace Qi
{
    /// <summary>
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        ///     Convert byte to string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToStringEx(this byte[] bytes, string format)
        {
            var stringbuilder = new StringBuilder(bytes.Length*2);
            foreach (byte byt in bytes)
            {
                stringbuilder.Append(byt.ToString(format));
            }
            return stringbuilder.ToString();
        }

        /// <summary>
        ///     use x2 format to conver byte;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToStringEx(this byte[] bytes)
        {
            return bytes.ToStringEx("X2");
        }

        /// <summary>
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string binary)
        {
            int len = binary.Length/2;
            var ret = new byte[len];
            for (int i = 0; i < len; i++)
                ret[i] = (byte) Convert.ToInt32(binary.Substring(i*2, 2), 16);
            return ret;
        }
    }
}
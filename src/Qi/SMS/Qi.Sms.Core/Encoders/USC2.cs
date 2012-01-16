using System.Globalization;
using System.Text;

namespace Qi.Sms.Encoders
{
    public class Usc2 : IEncoding
    {
        public string Encode(string value)
        {
            var result = new StringBuilder();
            if (value.Length % 2 != 0)
            {
                value += "F";
            }

            var length = value.Length;
            for (var i = 1; i < length; i += 2) //奇偶互换
            {
                result.Append(value[i]);
                result.Append(value[i - 1]);
            }
            return result.ToString();
        }
        public string Decode(string s)
        {
            var buf = new byte[s.Length];
            for (int i = 0; i < s.Length; i += 4)
            {
                buf[i / 2] = byte.Parse(s.Substring(2 + i, 2), NumberStyles.AllowHexSpecifier);
                buf[i / 2 + 1] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return System.Text.Encoding.Unicode.GetString(buf);
        }
    }
}

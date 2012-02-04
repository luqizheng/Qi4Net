using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qi.Net
{
    public struct MacAddress
    {
        private readonly byte[] _taken;

        public MacAddress(byte a, byte b, byte c, byte d, byte e, byte f)
        {
            _taken = new byte[6];
            _taken[0] = a;
            _taken[1] = b;
            _taken[2] = c;
            _taken[3] = d;
            _taken[4] = e;
            _taken[5] = f;
        }

        public MacAddress(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (bytes.Length != 6)
                throw new ArgumentOutOfRangeException("bytes", "bytes's length must be  6.");
            _taken = bytes;
        }

        /// <summary>
        /// mac could be 00:00:00:00:00 or 11.11.11.11.11 or 
        /// 2222.2222.2222 or 010203040506
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static MacAddress Parse(string mac)
        {
            mac = Regex.Replace(mac, "\\W", "", RegexOptions.IgnoreCase);
            if (mac.Length < 12)
                throw new FormatException("mac address is not correct format.");
            var taken = new byte[6];
            int j = 0;
            for (int i = 0; i < mac.Length; i = i + 2)
            {
                var str = new string(new[] {mac[i], mac[i + 1]});
                taken[j] = Convert.ToByte(StringToByteArray(str));
                j++;
            }
            return new MacAddress(taken);
        }

        private static byte StringToByteArray(String hex)
        {
            int numberChars = hex.Length;
            var bytes = new byte[1];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes[0];
        }

        public override string ToString()
        {
            var result = new string[6];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToString(_taken[i], 16).PadLeft(2, '0').ToUpper();
            }
            return String.Join("-", result);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            bool result = base.Equals(obj);
            if (result)
                return true;

            if (obj.GetType() != typeof (MacAddress))
                return false;
            var iObj = (MacAddress) obj;
            return !_taken.Where((t, i) => iObj._taken[i] != t).Any();
        }

        public bool Equals(MacAddress other)
        {
            return Equals(other._taken, _taken);
        }

        public override int GetHashCode()
        {
            return _taken.Sum(a => a.GetHashCode());
        }

        public static bool operator ==(MacAddress a, MacAddress b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MacAddress a, MacAddress b)
        {
            return !(a == b);
        }
    }
}
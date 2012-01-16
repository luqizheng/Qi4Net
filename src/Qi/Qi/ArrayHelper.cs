using System;
using System.Collections.Generic;
using System.Text;

namespace Qi
{
    public static class ArrayHelper
    {
        /// <summary>
        /// a <see cref="System.String.Substring"/> method of array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int start, int length)
        {
            var s = "";
            if (array.Length - 1 < start)
            {
                throw new ArgumentOutOfRangeException("start", "start index is larger than array");
            }
            if ((start + length) > (array.Length))
            {
                throw new ArgumentOutOfRangeException("length", "length is out of rang.");
            }
            var result = new T[length];
            Array.Copy(array, start, result, 0, length);
            return result;
        }
        /// <summary>
        /// a <see cref="System.String.Substring"/> method of array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int start)
        {
            if (array.Length - 1 < start)
            {
                throw new ArgumentOutOfRangeException("start", "start index is larger than array");
            }
            return SubArray(array, start, array.Length - start);
        }
        /// <summary>
        /// Convert byte to string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToStringEx(this byte[] bytes, string format)
        {
            var stringbuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte byt in bytes)
            {
                stringbuilder.Append(byt.ToString(format));
            }
            return stringbuilder.ToString();
        }

        /// <summary>
        /// use x2 format to conver byte;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToStringEx(this byte[] bytes)
        {
            return bytes.ToStringEx("X2");
        }

        /// <summary>
        /// 把ary 按照每份拥有多个来分组数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ary"></param>
        /// <param name="maxLengthOfAry"></param>
        /// <returns></returns>
        public static IList<T[]> Split<T>(this T[] ary, int maxLengthOfAry)
        {
            int remider;
            return ary.Split(maxLengthOfAry, out remider);
        }

        /// <summary>
        /// 把Ary按照每分多少个Item进行切割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ary"></param>
        /// <param name="maxLengthOfAry">每个数组的最大个数</param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public static IList<T[]> Split<T>(this T[] ary, int maxLengthOfAry, out int remainder)
        {
            if (ary == null)
                throw new ArgumentNullException("ary");
            if (ary.Length == 0)
            {
                remainder = 0;
                return new List<T[]>();
            }

            int arySetLen = Math.DivRem(ary.Length, maxLengthOfAry, out remainder);
            var arySet = new List<T[]>(arySetLen + remainder);
            if (arySetLen != 0)
            {
                for (int i = 0; i < arySetLen; i++)
                {
                    var aryItems = new T[maxLengthOfAry];
                    Array.Copy(ary, maxLengthOfAry * i, aryItems, 0, maxLengthOfAry);
                    arySet.Add(aryItems);
                }
            }
            if (remainder != 0)
            {
                var aryItems = new T[remainder];
                Array.Copy(ary, maxLengthOfAry * arySetLen, aryItems, 0, remainder);
                arySet.Add(aryItems);
            }
            return arySet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ary"></param>
        /// <param name="dividEquallyNumber"></param>
        /// <returns></returns>
        public static T[][] DivEqual<T>(this T[] ary, int dividEquallyNumber)
        {
            if (ary == null)
            {
                throw new ArgumentNullException("ary");
            }
            if (ary.Length == 0)
            {
                var r = new T[0][];
                r[0] = new T[0];
                return r;
            }
            if (dividEquallyNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("dividEquallyNumber", "dividEquallyNumber should be more than 0.");
            }

            var result = new T[dividEquallyNumber < ary.Length ? dividEquallyNumber : ary.Length][];

            int remainder;
            int arylength = Math.DivRem(ary.Length, dividEquallyNumber, out remainder);
            int pos = 0;
            for (int i = 0; i < result.Length; i++)
            {
                int aryLength = remainder == 0 ? arylength : arylength + 1;
                var aryItem = new T[aryLength];
                result[i] = aryItem;
                Array.Copy(ary, pos, aryItem, 0, aryLength);
                if (remainder != 0)
                {
                    remainder--;
                }
                pos += aryItem.Length;
            }
            return result;
        }
    }
}
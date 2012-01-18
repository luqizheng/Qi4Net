using System;

namespace Qi
{
    public static class QMath
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="midPointRouting"></param>
        /// <returns></returns>
        public static decimal Ceilling(decimal i, int midPointRouting)
        {
            checked
            {
                /*
                 * 1）分为 整数和小数部分
                 * 2）小数部分，进行扩大为10的midPositRouting次方
                 * 3）把2）的结果进行celing,并且除以10的midPositRouting次方，变回原来的的小数。
                 * 4）整数部分 + 小数部分输出结果
                 * */
                if (midPointRouting < 0)
                    throw new ArgumentOutOfRangeException("midPointRouting", "midPointRouting should be >=0");
                if (midPointRouting == 0)
                    return Math.Ceiling(i);
                decimal integer = Math.Truncate(i); //1)                
                decimal dec = i - integer; //1)

                decimal vs = Convert.ToDecimal(Math.Pow(10, midPointRouting)); //2)

                decimal res = Math.Ceiling(dec * vs) / vs; //3）
                return integer + res; //4)
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="midPointRouting"></param>
        /// <returns></returns>
        public static decimal Truncate(decimal i, int midPointRouting)
        {
            if (midPointRouting < 0)
                throw new ArgumentOutOfRangeException("midPointRouting", "midPointRouting should be >=0");

            decimal integer = Math.Truncate(i);

            if (midPointRouting == 0)
            {
                return integer;
            }
            decimal dec = i - integer;
            decimal vs = Convert.ToDecimal(Math.Pow(10, midPointRouting));
            decimal res = Math.Truncate(dec * vs) / vs;
            return integer + res;
        }
    }
}
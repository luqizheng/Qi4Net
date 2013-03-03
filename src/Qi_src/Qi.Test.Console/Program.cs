using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qi.Test.Console
{
    public class Program
    {
        private enum A
        {
            [EnumDescription("appleKey", ResourceType = typeof(Qi.Test.Console.Resource))]
            Apple
        }
        static void Main(string[] args)
        {
            var a = A.Apple;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");
            var key = a.ToDescription();
            System.Console.WriteLine(key);
            System.Console.ReadLine();
        }
    }
}

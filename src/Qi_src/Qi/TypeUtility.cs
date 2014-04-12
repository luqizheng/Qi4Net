using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi
{
    public static class TypeUtility
    {
        public static bool IsInteger(ValueType value)
        {
            return (value is SByte || value is Int16 || value is Int32
                    || value is Int64 || value is Byte || value is UInt16
                    || value is UInt32 || value is UInt64);
        }

        public static bool IsFloat(ValueType value)
        {
            return (value is float | value is double | value is Decimal);
        }

        public static bool IsNumeric(Type value)
        {
            if (!(value == typeof(Byte) ||
                    value == typeof(Int16) ||
                    value == typeof(Int32) ||
                    value == typeof(Int64) ||
                    value == typeof(SByte) ||
                    value == typeof(UInt16) ||
                    value == typeof(UInt32) ||
                    value == typeof(UInt64) ||
                    value == typeof(Decimal) ||
                    value == typeof(Double) ||
                    value == typeof(Single)))
                return false;
            else
                return true;
        }

        public static bool IsBasicType(Type t)
        {
            return t == typeof(string) || t.IsEnum ||  t == typeof(bool) ||IsNumeric(t) ;
        }
    }
}

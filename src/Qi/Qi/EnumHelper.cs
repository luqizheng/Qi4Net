using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the objects from enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetObjects<T>()
        {
            Array s = Enum.GetValues(typeof(T));
            var result = new T[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                result[i] = (T)s.GetValue(i);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumValue)
        {
            return ToDescription(enumValue, Thread.CurrentThread.CurrentUICulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static List<string> ToDescriptionAry(this Enum enumValue)
        {
            return ToDescriptionAry(enumValue, Thread.CurrentThread.CurrentUICulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static List<string> ToDescriptionAry(Enum enumValue, CultureInfo cultureInfo)
        {
            Type enuType = enumValue.GetType();
            var hasFlags = enuType.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0;

            FieldInfo[] fieldinfos = enuType.GetFields();
            var ary = new List<string>(fieldinfos.Length);
            foreach (FieldInfo fieldInfo in fieldinfos)
            {
                if (fieldInfo.IsLiteral && IsMatch(enumValue, fieldInfo, hasFlags))
                {
                    string description = GetStringFromAttr(fieldInfo, cultureInfo);
                    ary.Add(description ?? fieldInfo.GetValue(null).ToString());
                }
            }
            return ary;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="cultureInfo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public static string ToDescription(Enum enumValue, CultureInfo cultureInfo)
        {
            List<string> ary = ToDescriptionAry(enumValue, cultureInfo);
            if (ary.Count != 0)
            {
                return String.Join(",", ary.ToArray());
            }
            return enumValue.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldinfo"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string GetStringFromAttr(FieldInfo fieldinfo, CultureInfo cultureInfo)
        {
            object[] attrs = fieldinfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (attrs.Length != 0)
            {
                var attr = (EnumDescriptionAttribute)attrs[0];
                if (attr.ResourceType != null)
                {
                    var resourceManager = new ResourceManager(attr.ResourceType);

                    string result = resourceManager.GetString(attr.Name, cultureInfo);
                    if (result != null)
                        return result;
                }
                else
                {
                    return attr.Name;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiEnumValue"></param>
        /// <param name="infos"></param>
        /// <param name="hasFlags"></param>
        /// <returns></returns>
        private static bool IsMatch(Enum multiEnumValue, FieldInfo infos, bool hasFlags)
        {
            var singleEnumValue = (Enum)infos.GetValue(null);
            if (singleEnumValue.Equals(multiEnumValue))
                return true;
            else if (!hasFlags)
                return false;

            int singleValue = Convert.ToInt32(singleEnumValue);
            int multiValue = Convert.ToInt32(multiEnumValue);
            if (singleValue != 0 && singleValue < multiValue)
            {
                return (singleValue & multiValue) == singleValue;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="descriptionString"></param>
        /// <returns></returns>
        public static T DescriptionToEnum<T>(string descriptionString)
        {
            descriptionString = descriptionString.ToLower().Trim();
            Type enumType = typeof(T);
            FieldInfo[] infos = enumType.GetFields();
            foreach (FieldInfo info in infos)
            {
                object[] descripts = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                if (descripts.Length != 0)
                {
                    if (((EnumDescriptionAttribute)descripts[0]).Name.ToLower() == descriptionString)
                    {
                        return (T)Enum.Parse(enumType, info.Name);
                    }
                }
            }

            throw new ArgumentOutOfRangeException(
                "descriptionString", descriptionString, "can't match enum type " + typeof(T).Name);
        }

        /// <summary>
        /// ×Ö·û´®×ªÃ¶¾Ù
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumExpress"></param>
        /// <returns></returns>
        public static T ToEnum<T>(string enumExpress)
        {
            return (T)Enum.Parse(typeof(T), enumExpress, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SortedDictionary<string, T> GetDescriptionList<T>()
        {
            return GetDescriptionList<T>(Thread.CurrentThread.CurrentUICulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static SortedDictionary<string, object> GetDescriptionList(Type enumType)
        {
            return GetDescriptionList(enumType, Thread.CurrentThread.CurrentUICulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static SortedDictionary<string, object> GetDescriptionList(Type enumType, CultureInfo cultureInfo)
        {
            FieldInfo[] fields = enumType.GetFields();
            var result = new SortedDictionary<string, object>();

            foreach (FieldInfo field in fields)
            {
                if (field.IsSpecialName)
                    continue;
                object value = field.GetValue(null);
                string descript = GetStringFromAttr(field, cultureInfo);
                result.Add(descript ?? value.ToString(), value);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static SortedDictionary<string, T> GetDescriptionList<T>(CultureInfo cultureInfo)
        {
            var resultItem = GetDescriptionList(typeof(T));

            var result = new SortedDictionary<string, T>();
            foreach (var item in resultItem.Keys)
            {
                result.Add(item, (T)resultItem[item]);
            }

            return result;
        }
    }
}
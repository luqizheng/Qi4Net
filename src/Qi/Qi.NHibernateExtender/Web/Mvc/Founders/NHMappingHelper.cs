using System;
using NHibernate.Type;
using Qi.NHibernate;

namespace Qi.Web.Mvc.Founders
{
    internal static class NHMappingHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valStrExpress"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">format exception </exception>
        public static object ConvertStringToObject(string valStrExpress, IType type)
        {
            var idType = type as NullableType;
            if (idType == null)
                throw new NhConfigurationException(
                    "Resource's Id only support mapping from NullableType in nhibernate.");
            try
            {
                return idType.FromStringValue(valStrExpress);
            }
            catch (FormatException ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        public static string[] ConvertToArray(string valStrJoinByComma)
        {
            if (valStrJoinByComma != null)
                return valStrJoinByComma.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return new string[0];
        }

        public static object[] ConvertStringToObjects(string valStrJoinByComma, IType type)
        {
         
            if (valStrJoinByComma != null)
            {
                string[] aryStr = valStrJoinByComma.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var result = new object[aryStr.Length];
                for (int i = 0; i < aryStr.Length; i++)
                {
                    try
                    {
                        result[i] = ConvertStringToObject(aryStr[i], type);
                    }
                    catch(FormatException ex)
                    {
                        throw new FormatException(ex.Message+",actualValue:"+aryStr+" target value type "+type.Name,ex);
                    }
                    catch(ArgumentException ex)
                    {
                        throw new FormatException(ex.Message + ",actualValue:" + aryStr + " target value type " + type.Name, ex);
                    }
                }
                return result;
            }
            return new object[0];
        }
    }
}
using System;
using NHibernate.Type;
using Qi.Nhibernates;

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
        public static object ConvertStringToObject(string valStrExpress, IType type)
        {
            var idType = type as NullableType;
            if (idType == null)
                throw new NhConfigurationException(
                    "Resource's Id only support mapping from NullableType in nhibernate.");
            return idType.FromStringValue(valStrExpress);
        }

        public static object[] ConvertStringToObjects(string valStrJoinByComma, IType type)
        {
            if (valStrJoinByComma != null)
            {
                string[] aryStr = valStrJoinByComma.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var result = new object[aryStr.Length];
                for (int i = 0; i < aryStr.Length; i++)
                {
                    result[i] = ConvertStringToObject(aryStr[i], type);
                }
                return result;
            }
            return new object[0];
        }
    }
}
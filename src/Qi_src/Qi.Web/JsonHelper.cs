﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace Qi.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static JsonContainer Create(Stream stream)
        {
            return JsonContainer.Create(stream);
        }

        public static JsonContainer Create(string json)
        {
            return JsonContainer.Create(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="knowTypes"></param>
        /// <returns></returns>
        public static string ToDataContactJson(object obj, params Type[] knowTypes)
        {
            DataContractSerializer dcs = knowTypes == null
                                             ? new DataContractSerializer(obj.GetType())
                                             : new DataContractSerializer(obj.GetType(), knowTypes);
            var memoryStream = new MemoryStream();
            dcs.WriteObject(memoryStream, obj);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public static string ToJson(this Dictionary<string, object> data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return ToJson(data, false);
        }
        /// <summary>
        /// 无法处理双向关联
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToJson(this IDictionary<string, object> data, bool format)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            string formatSymbol = format ? "\r\n" : "";

            var buffer = new StringBuilder(String.Format("{{{0}", formatSymbol));

            int i = 1;
            foreach (string jsonKeyName in data.Keys)
            {
                buffer.Append("\"").Append(jsonKeyName).Append("\":");

                var obj = data[jsonKeyName] as IDictionary<string, object>;
                buffer.Append(obj != null ? ToJson(obj, format) : ToJson(data[jsonKeyName]));

                if (i != data.Count)
                {
                    buffer.Append(",").Append(formatSymbol); 
                }
                i++;
            }
            buffer.AppendFormat("}}{0}", formatSymbol);
            return buffer.ToString();
        }
    }
}
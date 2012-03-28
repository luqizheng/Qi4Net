using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Qi.Web.JsonContainers;

namespace Qi.Web
{
    /// <summary>
    /// </summary>
    public sealed class JsonContainer
    {
        /// <summary>
        /// </summary>
        private readonly Dictionary<string, object> _content = new Dictionary<string, object>();


        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContainer"/> class. 
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public JsonContainer(Dictionary<string, object> content)
        {
            _content = content;
        }

        public JsonContainer()
        {
            _content=new Dictionary<string, object>();
        }

        public string[] Keys
        {
            get { return _content.Keys.ToArray(); }
        }

        /// <summary>
        /// Get all sub key which value isn't JsonContaner
        /// </summary>
        public string[] AllSubKeys
        {
            get
            {
                var result = new List<string>();
                JoinKey(_content, null, result);
                return result.ToArray();
            }
        }

        public int Count
        {
            get { return _content.Count; }
        }

        private static void JoinKey(Dictionary<string, object> collection, string parentKey, List<string> result)
        {
            foreach (string key in collection.Keys)
            {
                var container = collection[key] as Dictionary<string, object>;
                if (container != null)
                {
                    JoinKey(container, parentKey == null ? key : string.Format("{0}.{1}", parentKey, key), result);
                }
                else
                {
                    result.Add(parentKey == null ? key : string.Format("{0}.{1}", parentKey, key));
                }
            }
        }

        public T To<T>(string key, Func<object, T> convert)
        {
            string theKey;
            bool isAry;
            Dictionary<string, object> current = FindContext(key, out theKey, out isAry);
            return convert(current[theKey]);
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public Int16 ToInt16(string key)
        {
            return To(key, Convert.ToInt16);
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public Int32 ToInt32(string key)
        {
            return To(key, Convert.ToInt32);
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public Int64 ToInt64(string key)
        {
            return To(key, Convert.ToInt64);
        }


        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public Boolean ToBoolean(string key)
        {
            return To(key, Convert.ToBoolean);
        }

        public Double ToDouble(string key)
        {
            return To(key, Convert.ToDouble);
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public UInt16 ToUInt16(string key)
        {
            return To(key, Convert.ToUInt16);
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public UInt32 ToUInt32(string key)
        {
            return To(key, Convert.ToUInt32);
        }

        public DateTime ToDateTime(string key)
        {
            return DateTime.Parse(ToString(key));
        }

        public DateTime ToDateTime(string key, string format, IFormatProvider provider, DateTimeStyles dateTimeStyles)
        {
            return DateTime.ParseExact(ToString(key), format, provider, dateTimeStyles);
        }

        public DateTime ToDateTime(string key, string format, IFormatProvider provider)
        {
            return DateTime.ParseExact(ToString(key), format, provider);
        }

        public DateTime ToDateTime(string key, string[] format, IFormatProvider provider, DateTimeStyles dateTimeStyles)
        {
            return DateTime.ParseExact(ToString(key), format, provider, dateTimeStyles);
        }


        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public JsonContainer ToJsonContainer(string key)
        {
            return To(key, s => new JsonContainer((Dictionary<string, object>) s));
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public string ToString(string key)
        {
            return To(key, Convert.ToString);
        }

        public Guid ToGuid(string key)
        {
            return new Guid(ToString(key));
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="convert"> </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public T[] ToArray<T>(string key, Func<object, T> convert)
        {
            string theKey;
            bool isArray;
            Dictionary<string, object> content = FindContext(key, out theKey, out isArray);
            if (!isArray)
                throw new ArgumentException(key + " is not a array.");
            string keyName = GetKeyName(theKey);
            return Accesser.Create(content[keyName]).ToArray(convert);
        }


        /// <summary>
        /// </summary>
        /// <param name="keyPath">
        /// The keyPath.
        /// </param>
        /// <param name="lastKey">
        /// The last key of keyPath.
        /// </param>
        /// <param name="isArray"> </param>
        /// <returns>
        /// </returns>
        private Dictionary<string, object> FindContext(string keyPath, out string lastKey, out bool isArray)
        {
            string[] keys = keyPath.Split('.');
            lastKey = keys[keys.Length - 1];
            isArray = lastKey.Contains("[");
            return GetContextByKeyPath(keys);
        }

        /// <summary>
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// </returns>
        public static JsonContainer Create(Stream stream)
        {
            var body = new byte[stream.Length];
            stream.Read(body, 0, body.Length);
            return Create(Encoding.Default.GetString(body));
        }

        /// <summary>
        /// </summary>
        /// <param name="jsonData">
        /// The json data.
        /// </param>
        /// <returns>
        /// </returns>
        public static JsonContainer Create(string jsonData)
        {
            var s = new JavaScriptSerializer();
            var result = (Dictionary<string, object>) s.DeserializeObject(jsonData);
            return new JsonContainer(result);
        }

        public bool Contains(string key)
        {
            try
            {
                string theKey;
                bool isArray;

                Dictionary<string, object> content = FindContext(key, out theKey, out isArray);
                if (isArray)
                {
                    string keyName = GetKeyName(theKey);
                    int aryIndex = GetArrayIndexFromKey(theKey);
                    if (!content.ContainsKey(keyName))
                        return false;
                    return Accesser.Create(content[keyName]).Count >= aryIndex;
                }
                return content.ContainsKey(theKey);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        public void SetValue(string key, string val)
        {
            if (key == null) throw new ArgumentNullException("key");
            SetValue(key, new[] {val});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">key path such as a.b.c or a[0].b.c or a.b.c[] or a.b[1]</param>
        /// <param name="val"></param>
        public void SetValue(string key, string[] val)
        {
            string lastKey;
            bool isArray;

            Dictionary<string, object> content = FindContext(key, out lastKey, out isArray);

            if (isArray)
            {
                int aryIndex = GetArrayIndexFromKey(lastKey);
                lastKey = GetKeyName(lastKey);
                Accesser accesser = null;
                if (!content.ContainsKey(lastKey))
                {
                    var aryValues = new List<string>();
                    content.Add(lastKey, aryValues);
                    accesser = new ListAccesser(aryValues);
                }
                else
                {
                    accesser = Accesser.Create(content[lastKey]);
                }
                if (aryIndex == -1)
                {
                    aryIndex = accesser.Count;
                }

                accesser.Set(val, aryIndex);
            }
            else
            {
                if (content.ContainsKey(lastKey))
                    content[lastKey] = val[0];
                else
                    content.Add(lastKey, val[0]);
            }
        }

        private Dictionary<string, object> GetContextByKeyPath(string[] keySet)
        {
            Dictionary<string, object> content = _content;
            for (int index = 0; index < keySet.Length - 1; index++)
            {
                string keyPath = keySet[index];
                content = SetKeyPath(content, ref keyPath);
            }
            return content;
        }

        private static Dictionary<string, object> SetKeyPath(Dictionary<string, object> content, ref string keyPath)
        {
            bool isArray = keyPath.Contains("[");
            if (isArray)
            {
                int aryIndex = GetArrayIndexFromKey(keyPath);
                keyPath = GetKeyName(keyPath);
                return GetArrayContext(aryIndex, keyPath, content);
            }
            if (!content.ContainsKey(keyPath))
            {
                content.Add(keyPath, new Dictionary<string, object>());
            }
            return (Dictionary<string, object>) content[keyPath];
        }

        private static Dictionary<string, object> GetArrayContext(int aryIndex, string key,
                                                                  Dictionary<string, object> content)
        {
            List<Dictionary<string, object>> valResult;
            if (!content.ContainsKey(key))
            {
                content.Add(key, valResult = new List<Dictionary<string, object>>());
            }
            else
            {
                valResult = (List<Dictionary<string, object>>) content[key];
            }

            while (aryIndex >= valResult.Count)
            {
                valResult.Add(new Dictionary<string, object>());
            }
            return valResult[aryIndex];
        }

        private static string GetKeyName(string key)
        {
            return key.Substring(0, key.IndexOf('['));
        }

        /// <summary>
        /// get index from json path such as a.b.c or a[0].b.c or a.b.c[] or a.b[1]
        /// if a.b.c[] means auto increate index.
        /// </summary>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        private static int GetArrayIndexFromKey(string keyPath)
        {
            if (keyPath.Contains("[]"))
                return -1;
            return Convert.ToInt32(Regex.Match(keyPath, @"\d+").Value);
        }

        public static JsonContainer ConvertToJsonContainer(object arg)
        {
            return new JsonContainer((Dictionary<string, object>) arg);
        }

        public string ToJson()
        {
            return _content.ToJson(false);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

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

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        public Int16 ToInt16(string key)
        {
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToInt16(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToInt32(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToInt64(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToBoolean(current._content[theKey]);
        }


        private Dictionary<string, object>[] ToJsoncContainer(object value)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (JsonContainer item in ((Array)value))
            {
                result.Add(item._content);
            }
            return result.ToArray();
        }

        public Double ToDouble(string key)
        {
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToDouble(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToUInt16(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToUInt32(current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return new JsonContainer((Dictionary<string, object>)current._content[theKey]);
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
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return Convert.ToString(current._content[theKey]);
        }

        public Guid ToGuid(string key)
        {
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return new Guid(current._content[theKey].ToString());
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
        public T[] ToArray<T>(string key)
        {
            string theKey;
            JsonContainer lastContainer = AnaylzTheKey(key, out theKey, this);
            object objArys = lastContainer._content[theKey];
            var array = objArys as Array;
            if (array != null)
            {
                return From<T>(array, array.Length);
            }
            var a = objArys as IList;
            if (a != null)
                return From<T>(a, a.Count);
            throw new ApplicationException(key + " isn't array or list.");
        }

        private T[] From<T>(IEnumerable array, int length)
        {
            Array result = new T[length];
            bool isJsonContainer = typeof(T) == typeof(JsonContainer);

            int i = 0;
            foreach (object item in array)
            {
                try
                {
                    if (isJsonContainer)
                    {
                        result.SetValue(new JsonContainer((Dictionary<string, object>)item), i);
                    }
                    else
                    {
                        result.SetValue((T)item, i);
                    }
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException("can't cast " + item + " to " + typeof(T));
                }
                i++;
            }
            return (T[])result;
        }


        private static JsonContainer BuildFromKey(string keyPath, out string lastKey, JsonContainer current)
        {
            string[] keys = keyPath.Split('.');
            if (keys.Length == 1)
            {
                lastKey = keys[0];
                return current;
            }
            lastKey = keys[keys.Length - 1];
            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!current._content.ContainsKey(keys[i]))
                {
                    current._content.Add(keys[i], new Dictionary<string, object>());
                }
                current = current.ToJsonContainer(keys[i]);
            }
            return current;
        }


        /// <summary>
        /// </summary>
        /// <param name="keyPath">
        /// The keyPath.
        /// </param>
        /// <param name="lastKey">
        /// The last key of keyPath.
        /// </param>
        /// <returns>
        /// </returns>
        [Obsolete]
        private JsonContainer AnaylzTheKey(string keyPath, out string lastKey, JsonContainer current)
        {
            string[] keys = keyPath.Split('.');
            Dictionary<string, object> content = GetContextByKeyPath(keys);
            lastKey = keys[keys.Length - 1];
            return new JsonContainer(content);
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
            var result = (Dictionary<string, object>)s.DeserializeObject(jsonData);
            return new JsonContainer(result);
        }

        public bool Contains(string key)
        {
            try
            {
                string theKey;
                JsonContainer lastContainer = AnaylzTheKey(key, out theKey, this);
                return lastContainer._content.ContainsKey(theKey);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        public object ToObject(string key)
        {
            string theKey;
            JsonContainer current = AnaylzTheKey(key, out theKey, this);
            return current._content[theKey];
        }
        public void SetValue(string key, string val)
        {
            SetValue(key, new[] { key });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">key path such as a.b.c or a[0].b.c or a.b.c[] or a.b[1]</param>
        /// <param name="val"></param>
        public void SetValue(string key, string[] val)
        {
            string[] keySet = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string lastKey = keySet[keySet.Length - 1];

            bool isArray = lastKey.EndsWith("]");

            Dictionary<string, object> content = GetContextByKeyPath(keySet);
            if (isArray)
            {
                int aryIndex = GetIndex(lastKey);
                lastKey = GetKeyName(lastKey);
                List<string> aryValues = null;
                if (!content.ContainsKey(lastKey))
                {
                    aryValues = new List<string>();
                    content.Add(lastKey, aryValues);
                }
                else
                {
                    aryValues = content[lastKey] as List<string>;
                    ;
                }
                if (aryIndex == -1)
                {
                    aryIndex = aryValues.Count;
                }
                while (aryIndex > aryValues.Count)
                {
                    aryValues.Add(null);
                }
                aryValues.AddRange(val);
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
                int aryIndex = GetIndex(keyPath);
                keyPath = GetKeyName(keyPath);
                return GetArrayContext(aryIndex, keyPath, content);
            }
            if (!content.ContainsKey(keyPath))
            {
                content.Add(keyPath, new Dictionary<string, object>());
            }
            return (Dictionary<string, object>)content[keyPath];
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
                valResult = (List<Dictionary<string, object>>)content[key];
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
        private static int GetIndex(string keyPath)
        {
            if (keyPath.Contains("[]"))
                return -1;
            return Convert.ToInt32(Regex.Match(keyPath, @"\d+").Value);
        }
    }
}
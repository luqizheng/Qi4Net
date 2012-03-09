using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        public void SetVal(string key, object val)
        {
            bool isJsonContainer = false;
            bool isArray = false;
            if (val != null)
            {
                isJsonContainer = val is JsonContainer;
                Type type = val.GetType();
                if (type.IsArray)
                {
                    isJsonContainer = type.GetElementType() == typeof(JsonContainer);
                    if (!type.GetElementType().IsValueType && type.GetElementType() != typeof(string) &&
                        isJsonContainer)
                    {
                        throw new ArgumentException(
                            "can not suppport array complex object,please use jsonContainer to instead.");
                    }
                    isArray = true;
                }
                else if (!(val is ValueType) && !(val is string) && !(val is JsonContainer))
                {
                    throw new ArgumentException("can not suppport complex object,please use JsonContainer to instead.");
                }
            }


            string theKey;
            JsonContainer current = BuildFromKey(key, out theKey, this);
            if (!isArray)
            {
                if (!current.Contains(theKey))
                {
                    current._content.Add(theKey, isJsonContainer ? ((JsonContainer)val)._content : val);
                }
                else
                {
                    current._content[theKey] = isJsonContainer ? ((JsonContainer)val)._content : val;
                }
            }
            else
            {
                if (!current.Contains(theKey))
                {
                    current._content.Add(theKey, isJsonContainer ? ToJsoncContainer(val) : val);
                }
                else
                {
                    current._content[theKey] = isJsonContainer ? ToJsoncContainer(val) : val;
                }
            }
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
                return From<T>(array,array.Length);
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
        private static JsonContainer AnaylzTheKey(string keyPath, out string lastKey, JsonContainer current)
        {
            string[] keys = keyPath.Split('.');
            if (keys.Length == 1)
            {
                lastKey = keyPath;
                if (!current._content.ContainsKey(lastKey))
                    throw new ArgumentOutOfRangeException(
                        String.Format("could not find {0}", lastKey));
                return current;
            }

            lastKey = keys[keys.Length - 1];
            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (current._content.ContainsKey(keys[i]))
                {
                    current = current.ToJsonContainer(keys[i]);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        String.Format("could not find '{0}' which defined in '{1}'", keys[i], keyPath));
                }
            }

            if (!current._content.ContainsKey(lastKey))
                throw new ArgumentOutOfRangeException(
                    String.Format("could not find '{0}' which defined in '{1}'", lastKey, keyPath));
            return current;
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
    }
}
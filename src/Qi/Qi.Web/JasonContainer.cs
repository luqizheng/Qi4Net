using System;
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
            this._content = content;
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
                    if (parentKey == null)
                        result.Add(key);
                    else
                        result.Add(string.Format("{0}.{1}", parentKey, key));
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
            return new JsonContainer((Dictionary<string, object>) current._content[theKey]);
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
            var r = (Array) lastContainer._content[theKey];
            if (typeof (T) == typeof (JsonContainer))
            {
                Array result = Array.CreateInstance(typeof (T), r.Length);
                int index = 0;
                foreach (var item in r.OfType<Dictionary<string, object>>())
                {
                    var jsonContainer = new JsonContainer(item);
                    result.SetValue(jsonContainer, index);
                    index++;
                }
                r = result;
            }

            var r1 = new T[r.Length];
            int i = 0;
            foreach (T item in r)
            {
                r1[i] = item;
                i++;
            }
            return r1;
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
            var result = (Dictionary<string, object>) s.DeserializeObject(jsonData);
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
    }
}
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Qi.IO.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="outputSteam"></param>
        public static void SerializeBinary(this object obj, Stream outputSteam)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(outputSteam, obj);
        }

        /// <summary>
        /// SerializeBinary to binary.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void SerializeBinary(this object obj, string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                obj.SerializeBinary(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectStream"></param>
        /// <returns></returns>
        public static T DeserializeBinary<T>(Stream objectStream)
        {
            IFormatter formatter = new BinaryFormatter();
            return (T) formatter.Deserialize(objectStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T DeserializeBinary<T>(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Can't unserialize from  file.", file);
            }
            using (FileStream stream = File.OpenRead(file))
            {
                return DeserializeBinary<T>(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="outputSteam"></param>
        public static void SerializerXml(this object obj, Stream outputSteam)
        {
            var ser = new XmlSerializer(obj.GetType());
            ser.Serialize(outputSteam, obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        public static void SerializerXml(this object obj, string file)
        {
            var ser = new XmlSerializer(obj.GetType());
            using (var stream = new FileStream(file, FileMode.OpenOrCreate))
            {
                ser.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializerXml(string file, Type type)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return DeserializerXml(stream, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializerXml(Stream stream, Type type)
        {
            var ser = new XmlSerializer(type);
            return ser.Deserialize(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T DeserializerXml<T>(string file)
        {
            return (T) DeserializerXml(file, typeof (T));
        }
    }
}
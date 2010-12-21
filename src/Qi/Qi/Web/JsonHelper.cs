using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace Qi.Web
{
    public static class JsonHelper
    {
        public static string ToJson(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static string ToDataContactJson(object obj, params Type[] knowTypes)
        {
            var dcs = knowTypes == null ? new DataContractSerializer(obj.GetType()) : new DataContractSerializer(obj.GetType(), knowTypes);
            var memoryStream = new MemoryStream();
            dcs.WriteObject(memoryStream, obj);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
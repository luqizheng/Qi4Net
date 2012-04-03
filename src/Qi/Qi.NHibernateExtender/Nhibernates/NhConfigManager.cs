using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Qi.Nhibernates
{
    public class NhConfigManager
    {
        private static readonly IDictionary<string, NhConfig> Cache = new Dictionary<string, NhConfig>();

        public static IEnumerable<string> SessionFactoryNames
        {
            get
            {
                if (Cache.Count == 0)
                {
                    foreach (NhConfig a in GetNHFileInfos())
                    {
                        Cache.Add(a.SessionFactoryName, a);
                    }
                }
                return Cache.Keys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<NhConfig> GetNHFileInfos()
        {
            string[] files = GetConfigurationFile();
            var result = new NhConfig[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new NhConfig(files[i]);
            }
            return result;
        }


        public static NhConfig GetNhConfig(string sessionFactory)
        {
            if (Cache.Count == 0)
            {
                lock (typeof (NhConfig))
                {
                    foreach (NhConfig a in GetNHFileInfos())
                    {
                        Cache.Add(a.SessionFactoryName, a);
                    }

                }
            }
            if (!Cache.ContainsKey(sessionFactory))
                return null;
            

            return Cache[sessionFactory];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetConfigurationFile()
        {
            string filePath = ConfigurationManager.AppSettings["nhConfig"];
            if (String.IsNullOrEmpty(filePath))
            {
                filePath = "~/Config/hibernate.cfg.config";
            }
            string[] ary = Regex.Split(filePath,"[,; ]");
            var result = new string[ary.Length];

            for (int i = 0; i < ary.Length; i++)
            {
                result[i] = ApplicationHelper.MapPath(ary[i]);
            }
            return result;
        }
    }
}
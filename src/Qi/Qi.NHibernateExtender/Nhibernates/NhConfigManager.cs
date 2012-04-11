using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qi.Nhibernates
{
    public class NhConfigManager
    {
        private static readonly IDictionary<string, INhConfig> Cache = new Dictionary<string, INhConfig>();

        public static string[] SessionFactoryNames
        {
            get
            {
                if (Cache.Count == 0)
                {
                    foreach (NhFileConfig a in GetNHFileInfos())
                    {
                        Cache.Add(a.SessionFactoryName, a);
                    }
                }
                return Cache.Keys.ToArray();
            }
        }
        public static bool Contains(string sessionFactoryName)
        {
            return Cache.ContainsKey(sessionFactoryName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<NhFileConfig> GetNHFileInfos()
        {
            string[] files = GetConfigurationFile();
            var result = new NhFileConfig[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new NhFileConfig(files[i]);
            }
            return result;
        }

        /// <summary>
        /// if can't found the NHConfig return false.
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <returns></returns>
        public static INhConfig GetNhConfig(string sessionFactory)
        {
            if (Cache.Count == 0)
            {
                lock (typeof(NhFileConfig))
                {
                    foreach (NhFileConfig a in GetNHFileInfos())
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
            string[] ary = Regex.Split(filePath, "[,; ]");
            var result = new string[ary.Length];

            for (int i = 0; i < ary.Length; i++)
            {
                result[i] = ApplicationHelper.MapPath(ary[i]);
            }
            return result;
        }
    }
}
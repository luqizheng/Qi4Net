using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qi.Nhibernates
{
    /// <summary>
    /// NhConfig manager.
    /// </summary>
    public class NhConfigManager
    {
        private static readonly IDictionary<string, INhConfig> Cache = new Dictionary<string, INhConfig>();

        /// <summary>
        /// Gets or sets the session factory names.
        /// </summary>
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

        /// <summary>
        /// Determine whether the configurations contains the session factory with the specified name.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">session factory name is empty of null</exception>
        public static bool Contains(string sessionFactoryName)
        {
            if (String.IsNullOrEmpty(sessionFactoryName))
                throw new ArgumentNullException("sessionFactoryName");
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
        ///<exception cref="NhConfigurationException">Can't find the sessionFactory</exception>
        public static INhConfig GetNhConfig(string sessionFactory)
        {
            if (Cache.Count == 0)
            {
                lock (typeof (NhConfigManager))
                {
                    foreach (NhFileConfig a in GetNHFileInfos())
                    {
                        Cache.Add(a.SessionFactoryName, a);
                    }
                }
            }
            if (!Cache.ContainsKey(sessionFactory))
                throw new NhConfigurationException("Can not find the sessionFactory named " + sessionFactory);


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
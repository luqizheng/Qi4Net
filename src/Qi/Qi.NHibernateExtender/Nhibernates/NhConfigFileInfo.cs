using System;
using System.Configuration;
using System.IO;
using System.Xml;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;

namespace Qi.Nhibernates
{
    public class NhConfigFileInfo
    {
        private readonly object _lockTable = 1;
        private ISessionFactory _factory;
        private Configuration _nhConfiguration;
        private string _sessionFactoryName;

        public NhConfigFileInfo(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Can't find the nhibernate config file.", file);
            CfgFile = file;
        }

        public NhConfigFileInfo(string sessionFactoryName, Configuration cfg)
        {
            if (sessionFactoryName == null) throw new ArgumentNullException("sessionFactoryName");
            if (cfg == null) throw new ArgumentNullException("cfg");
            _sessionFactoryName = sessionFactoryName;
            _nhConfiguration = cfg;
        }

        public string SessionFactoryName
        {
            get { return _sessionFactoryName ?? (_sessionFactoryName = GetSessionFactoryName(CfgFile)); }
        }

        public ISessionFactory SessionFactory
        {
            get
            {
                if (_factory == null || IsChanged)
                {
                    lock (_lockTable)
                    {
                        if (_factory == null)
                        {
                            _factory = NHConfiguration.BuildSessionFactory();
                        }
                        else if (IsChanged)
                        {
                            _factory.GetCurrentSession().Close();
                            _factory = NHConfiguration.BuildSessionFactory();
                        }
                    }
                }
                return _factory;
            }
        }

        public Configuration NHConfiguration
        {
            get
            {
                if (_nhConfiguration == null)
                {
                    lock (this)
                    {
                        if (_nhConfiguration == null)
                            _nhConfiguration = new Configuration();
                        _nhConfiguration.Configure(CfgFile);
                    }
                }
                return _nhConfiguration;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CfgFile { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifyTime { get; private set; }

        /// <summary>
        /// Get the value indecate file is changed or not.
        /// </summary>
        public bool IsChanged
        {
            get
            {
                if (CfgFile == null)
                    return false;

                DateTime time = File.GetLastWriteTime(CfgFile);
                if (ModifyTime != time)
                {
                    ModifyTime = time;
                    return true;
                }
                return false;
            }
        }

        public void Refresh()
        {
            if (IsChanged)
            {
                _sessionFactoryName = GetSessionFactoryName(CfgFile);
                lock (_lockTable)
                {
                    _factory = null;
                }
            }
        }

        public static NhConfigFileInfo[] GetNHFileInfos()
        {
            string[] files = GetConfigurationFile();
            var result = new NhConfigFileInfo[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new NhConfigFileInfo(files[i]);
            }
            return result;
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
            string[] ary = filePath.Split(',');
            var result = new string[ary.Length];

            for (int i = 0; i < ary.Length; i++)
            {
                result[i] = ApplicationHelper.MapPath(ary[i]);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetSessionFactoryName(string filePath)
        {
            using (XmlReader dom = XmlReader.Create(new StreamReader(filePath)))
            {
                while (dom.Read())
                {
                    //<session - factory name = "Main"
                    if (dom.IsStartElement("session-factory"))
                    {
                        return dom.GetAttribute("name") ?? "default";
                    }
                }
            }
            return "default";
        }
    }
}
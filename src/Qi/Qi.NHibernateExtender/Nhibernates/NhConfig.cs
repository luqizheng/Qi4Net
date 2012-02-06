using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;

namespace Qi.Nhibernates
{
    public class NhConfig
    {
        private readonly object _lockTable = 1;
        private ISessionFactory _factory;
        private Configuration _nhConfiguration;
        private string _sessionFactoryName;

        public NhConfig(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Can't find the nhibernate config file.", file);
            CfgFile = file;
        }

        public NhConfig(string sessionFactoryName, Configuration cfg)
        {
            if (sessionFactoryName == null) throw new ArgumentNullException("sessionFactoryName");
            if (cfg == null) throw new ArgumentNullException("cfg");
            _sessionFactoryName = sessionFactoryName;
            _nhConfiguration = cfg;
        }

        /// <summary>
        /// Gets the SessionFactoryName 
        /// </summary>
        public string SessionFactoryName
        {
            get { return _sessionFactoryName ?? (_sessionFactoryName = GetSessionFactoryName(CfgFile)); }
        }

        /// <summary>
        /// 
        /// </summary>
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
                            if (!_factory.IsClosed)
                            {
                                _factory.Close();
                            }
                            _factory = NHConfiguration.BuildSessionFactory();
                        }
                    }
                }
                return _factory;
            }
        }

        /// <summary>
        /// Gets the Nhibernate Configuration from this setting file.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NhConfig[] GetNHFileInfos()
        {
            string[] files = GetConfigurationFile();
            var result = new NhConfig[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new NhConfig(files[i]);
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

        /// <summary>
        /// It only set the property
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file)
        {
            const string fixcontent =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<hibernate-configuration xmlns=""urn:nhibernate-configuration-2.2"">
  <session-factory name=""{0}"">
      {1}
  </session-factory>
</hibernate-configuration>";
            var fileContent = new StringBuilder();

            foreach (var a in NHConfiguration.Properties)
            {
                fileContent.Append(String.Format(@"<property name=""{0}"">{1}</property>", a.Key, a.Value));
            }
            using (var writer = new StreamWriter(file, false))
            {
                writer.Write(String.Format(fixcontent, SessionFactoryName, fileContent));
                writer.Flush();
                writer.Close();
            }
            CfgFile = file;
            Refresh();
        }
    }
}
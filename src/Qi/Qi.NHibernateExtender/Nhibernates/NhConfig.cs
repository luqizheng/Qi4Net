using System;
using System.IO;
using System.Text;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;

namespace Qi.Nhibernates
{
    public class NhConfig
    {
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
        /// Gets the Nhibernate Configuration from this setting file.
        /// </summary>
        public Configuration NHConfiguration
        {
            get
            {
                if (_nhConfiguration == null || IsChanged)
                {
                    lock (this)
                    {
                        if (_nhConfiguration == null || IsChanged)
                        {
                            try
                            {
                                _nhConfiguration = new Configuration();
                                _nhConfiguration.Configure(CfgFile);
                            }
                            catch (Exception ex)
                            {
                                throw new NhConfigurationException(CfgFile + " found error.", ex);
                            }
                        }
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
        public ISessionFactory BuildSessionFactory()
        {
            try
            {
                return NHConfiguration.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                var msg = this.SessionFactoryName + " throw exception.";
                throw new NhConfigurationException(msg, ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            if (IsChanged)
            {
                _nhConfiguration = null;
                _sessionFactoryName = GetSessionFactoryName(CfgFile);
            }
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
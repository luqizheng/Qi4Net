using System;
using System.IO;
using System.Text;
using System.Xml;
using NHibernate.Cfg;

namespace Qi.Nhibernates
{
    public class NhFileConfig : NhConfig
    {
        public NhFileConfig(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Can't find the nhibernate config file.", file);
            CfgFile = file;
        }

        public NhFileConfig(string sessionFactoryName, Configuration cfg)
            : base(sessionFactoryName, cfg)
        {
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
        public override bool IsChanged
        {
            get
            {
                if (CfgFile == null)
                    return false;

                DateTime time = File.GetLastWriteTime(CfgFile);
                return ModifyTime != time;
            }
        }


        protected override void ResetToUnChanged()
        {
            ModifyTime = File.GetLastWriteTime(CfgFile);
        }

        protected override string GetSessionFactoryName()
        {
            using (XmlReader dom = XmlReader.Create(new StreamReader(CfgFile)))
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

        protected override Configuration BuildConfiguration()
        {
            var result = new Configuration();
            result.Configure(CfgFile);
            return result;
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
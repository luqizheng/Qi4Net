using System;
using NHibernate;

namespace Qi.Nhibernates
{
    public static class SessionManagerExtender
    {
        public static void ExecuteBy(this SessionManager instance, ISessionFactory templateSessionFactory, VoidFunc<string> execute)
        {
            string newKey = Guid.NewGuid().ToString();
            var srcKey = SessionManager.CurrentSessionFactoryKey;
            try
            {
                SessionManager.Add(newKey, templateSessionFactory);
                SessionManager.CurrentSessionFactoryKey = newKey;
                execute(newKey);
            }
            finally
            {
                SessionManager.CurrentSessionFactoryKey = srcKey;
                templateSessionFactory.Close();
                SessionManager.Remove(newKey);
            }
        }

        public static void TemplateSwitch(this SessionManager instance, string sessionFactory, VoidFunc<string> execute)
        {
            string srcKey = null;
            srcKey = SessionManager.CurrentSessionFactoryKey;
            try
            {
                execute(srcKey);
            }
            finally
            {
                SessionManager.CurrentSessionFactoryKey = srcKey;
            }
        }
    }
}
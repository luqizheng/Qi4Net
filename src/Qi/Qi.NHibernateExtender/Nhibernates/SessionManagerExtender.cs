using System;
using NHibernate;

namespace Qi.Nhibernates
{
    public static class SessionManagerExtender
    {
        public static void ExecuteBy(this SessionManager instance, ISessionFactory templateSessionFactory, VoidFunc<string> execute)
        {
            string newKey = Guid.NewGuid().ToString();
            try
            {
                SessionManager.Add(newKey, templateSessionFactory);
                SessionManager.CurrentSessionFactoryKey = newKey;
                execute(newKey);
            }
            finally
            {
                templateSessionFactory.Close();
                SessionManager.Remove(newKey);
            }
        }

        public static void TemplateSwitch(this SessionManager instance, string sessionFactory, VoidFunc<string> execute)
        {
            string srcKey = null;
            try
            {
                srcKey = SessionManager.CurrentSessionFactoryKey;
                execute(srcKey);
            }
            finally
            {
                SessionManager.CurrentSessionFactoryKey = srcKey;
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using NHibernate;

namespace Qi.Nhibernates
{
    public class SessionManager
    {
        internal static readonly SortedDictionary<string, SessionSegment> Factories =
            new SortedDictionary<string, SessionSegment>();


        private static string _defaultSessionFactoryKey = String.Empty;

        public static readonly SessionManager Instance = new SessionManager();

        public string CurrentSessionFactory
        {
            get { return (string) CallContext.GetData("session.key.factory."); }
            set { CallContext.SetData("session.key.factory.", value); }
        }

        /// <summary>
        /// default sesison factory key in the sessionFactory file.
        /// </summary>
        public static string DefaultSessionFactoryKey
        {
            get
            {
                if (_defaultSessionFactoryKey == String.Empty)
                {
                    lock (_defaultSessionFactoryKey)
                    {
                        if (_defaultSessionFactoryKey == String.Empty)
                        {
                            //Found the default Key
                            string[] names = NhConfigManager.SessionFactoryNames;
                            _defaultSessionFactoryKey = names[0];
                            for (int i = 1; i < names.Length; i++)
                            {
                                if (names[i] == "default")
                                {
                                    _defaultSessionFactoryKey = names[i];
                                }
                            }
                        }
                    }
                }
                return _defaultSessionFactoryKey;
            }
        }
        /// <summary>
        /// May be null, if you are not set the <see cref="CurrentSessionFactory"/> key
        /// </summary>
        /// <returns></returns>
        public ISession GetCurrentSession()
        {
            if (CurrentSessionFactory == null)
            {
                throw new SessionManagerException("CurrentSessionFactory is empty, can't get the current session.");
            }
            return GetSessionFactory(CurrentSessionFactory).CurrentSession;
        }


        /// <summary>
        /// gets the session factory by session factory name defined in the sessionFactory file.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="sessionFactoryName"/> is not exist in session manager</exception>
        public static SessionSegment GetSessionFactory(string sessionFactoryName)
        {
            // In order to lazy init the session.
            INhConfig nhFileConfig;
            if (!Factories.ContainsKey(sessionFactoryName))
            {
                nhFileConfig = NhConfigManager.GetNhConfig(sessionFactoryName);
                if (nhFileConfig == null)
                {
                    throw new ArgumentOutOfRangeException("sessionFactoryName",
                                                          string.Format("can't find the{0}session factory.",
                                                                        sessionFactoryName));
                }

                if (!Factories.ContainsKey(sessionFactoryName)) //double check, it may use in multi-thread.
                {
                    Add(nhFileConfig.SessionFactoryName, nhFileConfig.BuildSessionFactory());
                }
            }
            else
            {
                nhFileConfig = NhConfigManager.GetNhConfig(sessionFactoryName);
            }
            if (nhFileConfig != null && nhFileConfig.IsChanged)
            {
                Factories[sessionFactoryName] = new SessionSegment(nhFileConfig.BuildSessionFactory());
            }
            return Factories[sessionFactoryName];
        }


        /// <summary>
        /// add session facotry in SessionManager.
        /// </summary>
        /// <param name="sessionFacotryName"></param>
        /// <param name="sessionFactory"></param>
        /// <exception cref="ArgumentNullException">sessionFacotryName or sessionFactory is null</exception>
        /// <exception cref="SessionManagerException">SessionManager has this sessionFacotryName</exception>
        public static void Add(string sessionFacotryName, ISessionFactory sessionFactory)
        {
            if (String.IsNullOrEmpty(sessionFacotryName)) throw new ArgumentNullException("sessionFacotryName");
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            if (Factories.ContainsKey(sessionFacotryName))
                throw new SessionManagerException(string.Format("session factory name {0} has defined.",
                                                                sessionFacotryName));

            Factories.Add(sessionFacotryName, new SessionSegment(sessionFactory));
        }

        public static void Remove(string sessionFactoryName)
        {
            if (String.IsNullOrEmpty(sessionFactoryName)) throw new ArgumentNullException("sessionFactoryName");
            if (!Factories.ContainsKey(sessionFactoryName))
                throw new ArgumentNullException(string.Format("session factory name {0} is not defined.",
                                                              sessionFactoryName));
            Factories.Remove(sessionFactoryName);
        }
    }
}
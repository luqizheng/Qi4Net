using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Qi.NHibernateExtender
{
    internal class SessionInfo
    {
        public SessionInfo(string name,ISession session)
        {
            this.Name = name;
            this.Session = session;
        }
        public string Name { get; private set; }

        public ISession Session { get; private set; }
    }
}

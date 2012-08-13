using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.NHibernate
{
    [Serializable]
    public class SessionManagerException:ApplicationException
    {
        public SessionManagerException(string message):base(message)
        {
            
        }
    }
}

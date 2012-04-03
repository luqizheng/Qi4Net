using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Nhibernates
{
    [Serializable]
    public class NhConfigurationException : ApplicationException
    {
        public NhConfigurationException(string msg)
            : base(msg)
        {

        }

        public NhConfigurationException(string msg, Exception innerexception)
            : base(msg, innerexception)
        {

        }
    }
}

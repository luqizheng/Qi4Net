using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Nhibernates
{
    public class NhConfigurationException:ApplicationException
    {
        public NhConfigurationException(string msg):base(msg)
        {
            
        }
    }
}

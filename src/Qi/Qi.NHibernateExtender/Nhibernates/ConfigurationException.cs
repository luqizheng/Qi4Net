using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Nhibernates
{
    public class ConfigurationException:ApplicationException
    {
        public ConfigurationException(string msg):base(msg)
        {
            
        }
    }
}

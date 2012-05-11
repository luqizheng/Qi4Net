using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qi.Domain;

namespace LibA
{
    public class KeyValueMapping : DomainObject<KeyValueMapping,Guid>
    {
        public virtual Dictionary<string, object> Mapping { get; set; }
    }
}

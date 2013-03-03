using System;
using System.Collections.Generic;
using Qi.Domain;

namespace LibA
{
    public class KeyValueMapping : DomainObject<KeyValueMapping, Guid>
    {
        public virtual Dictionary<string, object> Mapping { get; set; }
    }
}
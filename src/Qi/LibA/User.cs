using System;
using Qi.Domain;

namespace LibA
{
    public class UserA : DomainObject<Guid>
    {
        public virtual string Name { get; set; }
        public virtual string LoginId { get; set; }
    }
}
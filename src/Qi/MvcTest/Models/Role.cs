using System;
using Qi.Domain;

namespace MvcTest.Models
{
    public class Role : DomainObject<Role, Guid>
    {
        protected Role()
        {
        }

        public Role(string name)
        {
            Name = name;
        }

        public string Name { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
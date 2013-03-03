using System;
using Qi.Domain;

namespace Mvc4Test.Models
{
    public class Role : DomainObject<Role, Guid>
    {
        public Role()
        {
        }

        public Role(Guid id)
        {
            this.Id = id;
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
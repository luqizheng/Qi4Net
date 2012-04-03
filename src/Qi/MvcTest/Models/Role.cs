using System;
using Qi.Domain;

namespace MvcTest.Models
{
    public class Role : DomainObject<Guid>
    {
        protected Role()
        {


        }

        public Role(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
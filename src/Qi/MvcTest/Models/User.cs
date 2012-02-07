using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using Qi.Domain;

namespace MvcTest.Models
{
    public class User : DomainObject<Guid>
    {
        private Iesi.Collections.Generic.ISet<Role> _roles;

        public string LoginId { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public Iesi.Collections.Generic.ISet<Role> Roles
        {
            get
            {
                return _roles ?? (_roles = new HashedSet<Role>());
            }

        }

        public override int GetHashCode()
        {
            return (Name + Password).GetHashCode();
        }
    }
}
using System;
using Iesi.Collections.Generic;
using Qi.Domain;

namespace MvcTest.Models
{
    public class User : DomainObject<Guid>
    {
        public User()
        {
            this.CreateTime = DateTime.Now;
        }
        private ISet<Role> _roles;

        public string LoginId { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public ISet<Role> Roles
        {
            get { return _roles ?? (_roles = new HashedSet<Role>()); }
        }

        /// <summary>
        /// Readonly 
        /// </summary>
        public DateTime CreateTime { get; private set; }
        public override int GetHashCode()
        {
            return (Name + Password).GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
using System;
using Qi.Domain;

namespace MvcTest.Models
{
    public class User : DomainObject<Guid>
    {
        public string LoginId { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public override int GetHashCode()
        {
            return (Name + Password).GetHashCode();
        }
    }
}
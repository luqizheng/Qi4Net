using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.Models
{
    public class AssignRoleModel
    {
        public User User { get; set; }
        public IList<Role> Roles { get; set; }
    }
}
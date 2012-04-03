using System.Collections.Generic;
using Qi.Web.Mvc.Founders;
using System.Linq;
namespace MvcTest.Models
{
    public class AssignRoleModel
    {
        [PropertyFounder("LoginId")]
        public User User { get; set; }

        [IdFounder]
        public IList<Role> Roles { get; set; }

        public override string ToString()
        {
            return this.User.LoginId + " has roles " + string.Join(",", this.Roles);

        }
    }
}
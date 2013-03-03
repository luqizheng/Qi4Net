using System.Collections.Generic;
using Qi.Web.Mvc.Founders;

namespace NHibernateMvc4Test.Models
{
    public class AssignRoleModel
    {
        public User User { get; set; }

        [IdFounder]
        public IList<Role> Roles { get; set; }

        public override string ToString()
        {
            return User.LoginId + " has roles " + string.Join(",", Roles);
        }
    }
}
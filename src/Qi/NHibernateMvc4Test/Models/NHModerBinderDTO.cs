using System.Collections.Generic;

namespace NHibernateMvc4Test.Models
{
    public class NHModerBinderDTO
    {
        public string Something { get; set; }
        public IList<Role> RoleList { get; set; }
    }
}
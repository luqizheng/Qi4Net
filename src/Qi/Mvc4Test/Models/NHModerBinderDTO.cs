using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc4Test.Models
{
    public class NHModerBinderDTO
    {
        public string Something { get; set; }
        public IList<Role> RoleList { get; set; }
    }
}
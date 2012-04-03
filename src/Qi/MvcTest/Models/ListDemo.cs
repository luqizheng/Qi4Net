using System.Collections.Generic;
using System.Text;
using Qi.Web.Mvc.Founders;

namespace MvcTest.Models
{
    public class ListDemo
    {
        [PropertyFounder("LoginId")]
        public User[] UsersByLoginId { get; set; }

        public User[] UsersById { get; set; }

        [IdFounder]
        public IList<Role> RolesByDeclareIdFounder { get; set; }

       
        public ISet<Role> RolesBySet { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();

            if (UsersByLoginId != null)
            {
                result.Append("Select users:");
                result.Append(string.Join(",", new List<User>(UsersByLoginId)));
            }
            if (RolesByDeclareIdFounder != null)
            {
                result.Append("select roles: " + string.Join(",", RolesByDeclareIdFounder));
            }
            if (result.Length == 0)
            {
                result.Append("Select nothing.");
            }
            return result.ToString();


        }
    }
}
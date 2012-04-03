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


        public Iesi.Collections.Generic.ISet<Role> RolesBySet { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            if (UsersById != null)
            {
                result.Append("select UserById:")
                    .AppendLine(string.Join(",", new List<User>(UsersById)));
            }
            if (UsersByLoginId != null)
            {
                result.Append("Select UsersByLoginId:");
                result.AppendLine(string.Join(",", new List<User>(UsersByLoginId)));
            }
            if (RolesByDeclareIdFounder != null)
            {
                result.AppendLine("select IList roles: " + string.Join(",", RolesByDeclareIdFounder));
            }
            if (RolesBySet != null)
            {
                result.AppendLine("select Iesi.Collection.ISet " + string.Join(",", RolesBySet));
            }
            if (result.Length == 0)
            {
                result.AppendLine("Select nothing.");
            }

            return result.ToString();


        }
    }
}
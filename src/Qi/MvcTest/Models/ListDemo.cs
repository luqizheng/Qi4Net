using System.Collections.Generic;
using System.Text;
using Qi.Web.Mvc.Founders;

namespace MvcTest.Models
{
    public class ListDemo
    {
        [PropertyFounder("LoginId")]
        public User[] Users { get; set; }

        [IdFounder]
        public IList<Role> Roles { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();

            if (Users != null)
            {
                result.Append("Select users:");
                result.Append(string.Join(",", new List<User>(Users)));
            }
            if (Roles != null)
            {
                result.Append("select roles: " + string.Join(",", Roles));
            }
            if (result.Length == 0)
            {
                result.Append("Select nothing.");
            }
            return result.ToString();


        }
    }
}
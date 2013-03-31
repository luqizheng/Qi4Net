using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Qi.Browsers
{
    public interface ICookie
    {
        void SetCookie(HttpCookie cookie);
        System.Web.HttpCookie GetCookie(string name)
        

    }
}

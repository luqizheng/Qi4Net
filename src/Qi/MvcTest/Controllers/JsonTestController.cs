using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Qi.Web;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    public class JsonTestController : Controller
    {
        //
        // GET: /JsonTest/

        public ActionResult Index()
        {
            return View();
        }
        [JsonContainerFilter]
        public ActionResult Test(JsonContainer j1, JsonContainer j2)
        {
            var a = new
                        {
                            j1 = new
                                   {
                                       value = j1.ToInt32("value"),
                                       name = j1.ToString("name"),
                                   },
                            j2 = new
                            {
                                value = j1.ToInt32("value"),
                                name = j1.ToString("name"),
                            }

                        };
            return Json(a,JsonRequestBehavior.AllowGet);

        }

    }
}

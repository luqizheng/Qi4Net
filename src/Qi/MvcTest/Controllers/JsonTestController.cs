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
        [JsonContainerFilter(true)]
        public ActionResult Test(JsonContainer data)
        {

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}

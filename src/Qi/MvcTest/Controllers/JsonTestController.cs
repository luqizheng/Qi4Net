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
        public ActionResult PureJson()
        {
            return View();
        }
        [JsonContainerFilter()]
        public ActionResult PureJsonTest(JsonContainer json)
        {
            var msg = "name:" + json.ToString("name") + "<br>,email:" + json.ToString("email");
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #region simple json directry submitted by jquery.
        public ActionResult Simple()
        {

            return View();
        }
        [JsonContainerFilter(true)]
        public ActionResult SimpleTest(JsonContainer data)
        {
            var msg = "name:" + data.ToString("name") + "<br>,email:" + data.ToString("email");
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}

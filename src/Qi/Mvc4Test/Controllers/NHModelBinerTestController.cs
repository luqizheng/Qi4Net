using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc4Test.Models;

namespace Mvc4Test.Controllers
{
    public class NHModelBinerTestController : Controller
    {
        //
        // GET: /NHModelBinerTest/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArrayBinder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ArrayBinder(Role[] roles)
        {
            return View();
        }

        public ActionResult DtoPropertyBinder()
        {
            throw new NotImplementedException();
        }

        public ActionResult ArrayProperty()
        {
            throw new NotImplementedException();
        }
    }
}

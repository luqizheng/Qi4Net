using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibA;
using Qi.Nhibernates;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    public class NHExtendTypeController : Controller
    {
        //
        // GET: /NHExtendType/
        [Session]
        public ActionResult ValueKeyTest()
        {
            KeyValueMapping a = new KeyValueMapping();
            a.Mapping = new Dictionary<string, object>();
            a.Mapping.Add("string", "ccc");
            a.Mapping.Add("dateTime", new DateTime(1980, 1, 1, 12, 30, 33));
            a.Mapping.Add("int", 1);
            a.Mapping.Add("Address", new Dictionary<string, object>()
                                           {
                                               {"Street","hah"},
                                               {"number",15},
                                               {"City","zhuhai"}
                                           });
            SessionManager.Instance.GetCurrentSession().SaveOrUpdate(a);
            SessionManager.Instance.GetCurrentSession().Flush();
            a = SessionManager.Instance.GetCurrentSession().Get<KeyValueMapping>(a.Id);


            return RedirectToAction("ValueKeyResult", new { id = a.Id });
        }
        [Session]
        public ActionResult ValueKeyResult(Guid? id)
        {
            var a = SessionManager.Instance.GetCurrentSession().Get<KeyValueMapping>(id.Value);

            return View("ValueKeyTest", a);
        }

    }
}

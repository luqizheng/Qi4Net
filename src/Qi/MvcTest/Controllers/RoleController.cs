using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;
using Qi.NHibernate;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    [Session(Transaction = true)]
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {

            var a = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            return View(a);
        }
        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(NHModelBinder))]Role[] roles)
        {
            return View(roles);
        }

        public ActionResult Edit(Guid? id)
        {
            var role = SessionManager.Instance.GetCurrentSession().Get<Role>(id.Value);
            return View(role);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(NHModelBinder))]Role role)
        {
            SessionManager.Instance.GetCurrentSession().SaveOrUpdate(role);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(NHModelBinder))]Role role)
        {
            SessionManager.Instance.GetCurrentSession().SaveOrUpdate(role);
            return RedirectToAction("Index");
        }

    }
}

using System;
using System.Collections.Generic;

using System.Web.Mvc;
using Mvc4Test.Models;
using Qi.NHibernate;
using Qi.Web.Mvc;

namespace Mvc4Test.Controllers
{
    [Session]
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
            IList<Role> a = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            return View(a);
        }

        [HttpPost]
        public ActionResult ArrayBinder(Role[] roles)
        {
            foreach (Role role in roles)
            {
                SessionManager.Instance.GetCurrentSession().SaveOrUpdate(role);
            }
            return RedirectToAction("ArrayBinder");
        }

        public ActionResult DtoPropertyBinder()
        {
            IList<Role> a = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            return View(new NHModerBinderDTO
                {
                    RoleList = a,
                    Something = DateTime.Now.ToString("hh:mm:ss")
                });
        }

        [HttpPost]
        public ActionResult DtoPropertyBinder(NHModerBinderDTO dto)
        {
            foreach (Role role in dto.RoleList)
            {
                SessionManager.Instance.GetCurrentSession().SaveOrUpdate(role);
            }
            return RedirectToAction("DtoPropertyBinder");
        }

        public ActionResult ArrayProperty()
        {
            throw new NotImplementedException();
        }

        public ActionResult DtoListRefereOnly()
        {
            var a = TempData["roles"] as IList<Role>;
            if (a == null)
                a = new List<Role>();
            return View(new NHModerBinderDTO
                {
                    RoleList = a,
                    Something = DateTime.Now.ToString("hh:mm:ss")
                });
        }

        [HttpPost]
        public ActionResult DtoListRefereOnly(NHModerBinderDTO dto)
        {
            TempData["roles"] = dto.RoleList;
            if (Request.IsAjaxRequest())
            {
                return Json("ok");
            }
            return RedirectToAction("DtoListRefereOnly");
        }
    }
}
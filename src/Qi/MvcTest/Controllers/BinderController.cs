using System.Collections.Generic;
using System.Web.Mvc;
using MvcTest.Models;
using Qi.Nhibernates;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    [Session]
    public class BinderController : Controller
    {
        //
        // GET: /BinderTest/

        public ActionResult DtoListDemo()
        {
            IList<Role> roles = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            ViewData["roles"] = roles;
            ViewData["users"] = SessionManager.Instance.GetCurrentSession().CreateCriteria<User>().List<User>();
            return View();
        }

        [HttpPost]
        public ActionResult DtoListDemo([ModelBinder(typeof(NHModelBinder))]ListDemo model)
        {
            ViewData["roles"] = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            ViewData["users"] = SessionManager.Instance.GetCurrentSession().CreateCriteria<User>().List<User>();
            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MvcTest.Models;
using NHibernate.Criterion;
using Qi.Nhibernates;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    [Session]
    public class UserController : Controller
    {

        public ActionResult Index()
        {
            IList<User> r = SessionManager.Instance.CurrentSession.CreateCriteria<User>().List<User>();
            return View(r);
        }


        public ActionResult Edit(Guid? id)
        {
            var r = SessionManager.Instance.CurrentSession.Load<User>(id.Value);
            return View(r);
        }


        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(NHModelBinder))] User user)
        {
            SessionManager.Instance.CurrentSession.SaveOrUpdate(user);
            return RedirectToAction("Index");
        }


        public ActionResult ChangePassword(Guid? id)
        {
            var r = SessionManager.Instance.CurrentSession.Load<User>(id.Value);
            var v = new ChangeUserPasswordModel
                        {
                            User = r
                        };
            return View(v);
        }


        [HttpPost]
        public ActionResult ChangePassword(
            [ModelBinder(typeof(NHModelBinder))] ChangeUserPasswordModel changeUserPasswordModel)
        {
            if (ModelState.IsValid)
            {
                changeUserPasswordModel.User.Password = changeUserPasswordModel.NewPassword;
                SessionManager.Instance.CurrentSession.SaveOrUpdate(changeUserPasswordModel.User);
                return RedirectToAction("Index");
            }
            return View(changeUserPasswordModel);
        }

        [HttpPost]
        public ActionResult AssignRole([ModelBinder(typeof(NHModelBinder))]User u, Guid[] setRoles)
        {
            u.Roles.Clear();
            foreach (var roleId in setRoles)
            {
                var role = SessionManager.Instance.CurrentSession.Get<Role>(roleId);
                u.Roles.Add(role);
            }
            return RedirectToAction("Index");
        }

        public ActionResult AssignRole(string id)//user's loginid
        {
            ViewBag.Roles = SessionManager.Instance.CurrentSession.CreateCriteria<Role>().List<Role>();
            var user =
                SessionManager.Instance.CurrentSession.CreateCriteria<User>().Add(
                    Restrictions.Eq(Projections.Property<User>(s => s.LoginId), id))
                    .UniqueResult<User>();
            return View(user);
        }
    }
}


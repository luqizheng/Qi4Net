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

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(NHModelBinder))] User user)
        {
            if (this.ModelState.IsValid)
            {
                SessionManager.Instance.CurrentSession.SaveOrUpdate(user);
                RedirectToAction("Index");
            }

            return View(user);
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
        public ActionResult AssignRole([ModelBinder(typeof(NHModelBinder))]AssignRoleModel u)
        {
            u.User.Roles.Clear();
            u.User.Roles.AddAll(u.Roles);
            SessionManager.Instance.CurrentSession.SaveOrUpdate(u.User);
            return RedirectToAction("Index");
        }

        public ActionResult AssignRole(string id)//user's loginid
        {
            ViewBag.Roles = SessionManager.Instance.CurrentSession.CreateCriteria<Role>().List<Role>();
            var user =
                SessionManager.Instance.CurrentSession.CreateCriteria<User>().Add(
                    Restrictions.Eq(Projections.Property<User>(s => s.LoginId), id))
                    .UniqueResult<User>();
            return View(new AssignRoleModel()
                            {
                                User = user,
                                Roles = new List<Role>(user.Roles)
                            });
        }
    }
}


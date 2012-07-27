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
    public class UserController : AsyncController
    {
        [Session(true, "readonly")]
        public ActionResult Index()
        {
            IList<User> r = SessionManager.Instance.GetCurrentSession().CreateCriteria<User>().List<User>();
            ViewData["view"] = SessionManager.Instance.CurrentSessionFactoryName;
            return View(r);
        }


        public ActionResult Edit(Guid? id)
        {
            var r = SessionManager.Instance.GetCurrentSession().Load<User>(id.Value);
            ViewData["view"] = SessionManager.Instance.CurrentSessionFactoryName;
            return View(r);
        }

        public ActionResult Delete(Guid? id)
        {
            var session = SessionManager.Instance.GetCurrentSession();
            session.Delete(session.Load<User>(id));
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(NHModelBinder))] User user)
        {
            if (ModelState.IsValid)
            {
                SessionManager.Instance.GetCurrentSession().SaveOrUpdate(user);
                SessionManager.Instance.GetCurrentSession().Flush();
                return RedirectToAction("Index");
            }

            return View(user);
        }


        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(NHModelBinder))] User user)
        {
            SessionManager.Instance.GetCurrentSession().SaveOrUpdate(user);
            SessionManager.Instance.GetCurrentSession().Flush();
            return RedirectToAction("Index");
        }


        public ActionResult ChangePassword(Guid? id)
        {
            var r = SessionManager.Instance.GetCurrentSession().Load<User>(id.Value);
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
                SessionManager.Instance.GetCurrentSession().SaveOrUpdate(changeUserPasswordModel.User);
                return RedirectToAction("Index");
            }
            return View(changeUserPasswordModel);
        }

        [HttpPost]
        public ActionResult AssignRole([ModelBinder(typeof(NHModelBinder))] AssignRoleModel u)
        {
            u.User.Roles.Clear();
            if (u.Roles != null)
            {
                u.User.Roles.AddAll(u.Roles);
            }
            SessionManager.Instance.GetCurrentSession().SaveOrUpdate(u.User);
            return RedirectToAction("Index");
        }

        public ActionResult AssignRole(string id) //user's loginid
        {
            ViewBag.Roles = SessionManager.Instance.GetCurrentSession().CreateCriteria<Role>().List<Role>();
            var user =
                SessionManager.Instance.GetCurrentSession().CreateCriteria<User>().Add(
                    Restrictions.Eq(Projections.Property<User>(s => s.LoginId), id))
                    .UniqueResult<User>();
            return View(new AssignRoleModel
                            {
                                User = user,
                                Roles = new List<Role>(user.Roles)
                            });
        }
    }
}
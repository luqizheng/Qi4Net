using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MvcTest.Models;
using Qi.Nhibernates;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    public class UserController : Controller
    {
      
        [Session]
        public ActionResult Index()
        {
            IList<User> r = SessionManager.Instance.CurrentSession.CreateCriteria<User>().List<User>();
            return View(r);
        }

        [Session]
        public ActionResult Edit(Guid? id)
        {
            var r = SessionManager.Instance.CurrentSession.Load<User>(id.Value);
            return View(r);
        }


        [HttpPost, Session]
        public ActionResult Edit([ModelBinder(typeof (NHModelBinder))] User user)
        {
            SessionManager.Instance.CurrentSession.SaveOrUpdate(user);
            return RedirectToAction("Index");
        }

        [Session]
        public ActionResult ChangePassword(Guid? id)
        {
            var r = SessionManager.Instance.CurrentSession.Load<User>(id.Value);
            var v = new ChangeUserPasswordModel
                        {
                            User = r
                        };
            return View(v);
        }


        [HttpPost, Session]
        public ActionResult ChangePassword(
            [ModelBinder(typeof (NHModelBinder))] ChangeUserPasswordModel changeUserPasswordModel)
        {
            if (ModelState.IsValid)
            {
                changeUserPasswordModel.User.Password = changeUserPasswordModel.NewPassword;
                SessionManager.Instance.CurrentSession.SaveOrUpdate(changeUserPasswordModel.User);
                return RedirectToAction("Index");
            }
            else
            {
                return View(changeUserPasswordModel);
            }
        }
    }
}

}
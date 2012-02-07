﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;
using Qi.Nhibernates;
using Qi.Web.Mvc;

namespace MvcTest.Controllers
{
    [Session]
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {

            var a = SessionManager.Instance.CurrentSession.CreateCriteria<Role>().List<Role>();
            return View(a);
        }

        public ActionResult Edit(Guid? id)
        {
            var role = SessionManager.Instance.CurrentSession.Get<Role>(id.Value);
            return View(role);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(NHModelBinder))]Role role)
        {
            SessionManager.Instance.CurrentSession.SaveOrUpdate(role);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(NHModelBinder))]Role role)
        {
            SessionManager.Instance.CurrentSession.SaveOrUpdate(role);
            return RedirectToAction("Index");
        }

    }
}
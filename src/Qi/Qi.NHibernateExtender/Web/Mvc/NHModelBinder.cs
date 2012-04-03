using System;
using System.Linq;
using System.Web.Mvc;
using NHibernate;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// ModelBinder runs earlier than SessionAttribute, so this instance have to open Session by itself 
    /// and close it by SessionAttribute(because the entity may be lazy entity).
    /// if the controller has any SessionAttribute in actions or Controller, it have to close it in ActionEnding.
    /// </summary>
    public class NHModelBinder : NHModelBinderBase
    {
        protected override ISession CurrentSession(Type entityType)
        {
            return SessionManager.Instance.GetCurrentSession(entityType);
        }

        protected override ISessionFactory CurrentSessionFactory(Type entityType)
        {
            ISessionFactory sessionFactory;
            return SessionManager.Instance.TryGetSessionFactory(entityType, out sessionFactory) ? sessionFactory : null;
        }

        /// <summary>
        /// Find Session Attribute in the Action or Controller.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        protected override void Initilize(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var reflectedControllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            //find on Action, sessionAttribute 's priority on action is heiher than on controller.
            string actionname = controllerContext.RouteData.Values["Action"].ToString();
            ActionDescriptor action = reflectedControllerDescriptor.FindAction(controllerContext, actionname);

            //Find session attribute on the action.
            object[] customAttributes = action.GetCustomAttributes(typeof(SessionAttribute), true);
            if (customAttributes.Cast<SessionAttribute>().Any(s => s.Enable))
            {
                SessionManager.Instance.InitSession();
                return;
            }

            //try to find attribute on Controller.
            customAttributes = controllerContext.Controller.GetType().GetCustomAttributes(typeof(SessionAttribute),
                                                                                          true);
            if (customAttributes.Cast<SessionAttribute>().Any(s => s.Enable))
            {
                SessionManager.Instance.InitSession();
                return;
            }

            if (customAttributes.Length == 0)
                throw new NHModelBinderException(
                    "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");
        }
    }
}
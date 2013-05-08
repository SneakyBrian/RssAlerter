using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using RssAlerter.Mvc.Web.Models.Persistence;

namespace RssAlerter.Mvc.Web.Controllers
{
    public abstract class BaseDocumentStoreController : Controller
    {
        public IDocumentSession DocumentSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            this.DocumentSession = DataDocumentStore.Instance.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            if (this.DocumentSession != null && filterContext.Exception == null)
                this.DocumentSession.SaveChanges();
            this.DocumentSession.Dispose();
            base.OnActionExecuted(filterContext);
        }
    }
}
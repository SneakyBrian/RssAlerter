using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RssAlerter.Models.AlertBoards;
using DapperExtensions;
using RssAlerter.Mvc.Web.Models;
using RssAlerter.Mvc.Web.Models.Persistence;
using RssAlerter.Mvc.Web.Models.Persistence.Extensions;

namespace RssAlerter.Mvc.Web.Controllers
{
    [Authorize]
    public class AlertController : Controller
    {
        private RssAlerterDB db = new RssAlerterDB();

        //
        // GET: /Alert/

        public ActionResult Index(int id)
        {
            var alertboard = db.Get<AlertBoard>(id);
            //db.Load(alertboard);

            var alertBoardUser = db.GetCurrentUser();

            if (alertboard.UserId != alertBoardUser.Id)
                return RedirectToAction("Index");

            return View(alertboard);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

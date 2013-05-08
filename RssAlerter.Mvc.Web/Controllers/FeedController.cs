using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Configuration;
using System.Web.Configuration;
using RssAlerter.Models.AlertSources;
using RssAlerter.Mvc.Web.Models.Persistence;

namespace RssAlerter.Mvc.Web.Controllers
{
    public class FeedController : Controller
    {
        private RssAlerterDB db = new RssAlerterDB();

        //
        // GET: /Feed/

        public JsonResult GetAll()
        {
            var feedList = db.GetList<RssFeedAlertSource>().Select(rss => rss.FeedUri.ToString()).ToArray();

            return Json(feedList, JsonRequestBehavior.AllowGet);

            //var opmlDoc = XDocument.Load(WebConfigurationManager.AppSettings["OpmlPath"]);
            //var feedList = opmlDoc.Descendants("outline").Select(e => new Uri(e.Attribute("xmlUrl").Value)).ToArray();
            //return Json(feedList, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

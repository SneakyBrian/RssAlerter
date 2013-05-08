using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RssAlerter.Models.AlertSources;
using System.Web.Security;
using RssAlerter.Models.AlertBoards;
using RssAlerter.Mvc.Web.Models;
using DapperExtensions;
using RssAlerter.Mvc.Web.Models.Persistence;
using RssAlerter.Mvc.Web.Models.Persistence.Extensions;

namespace RssAlerter.Mvc.Web.Controllers
{ 
    [Authorize]
    public class RssFeedAdminController : Controller
    {
        private RssAlerterDB db = new RssAlerterDB();

        //
        // GET: /Admin/

        public ViewResult Index()
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssFeeds = db.GetList<RssFeedAlertSource>(alertBoardUser.RssFeedIds);

            return View(rssFeeds);
        }

        //
        // GET: /Admin/Details/5

        public ViewResult Details(string id)
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssFeed = db.Get<RssFeedAlertSource>(id);

            return View(rssFeed);
        }

        //
        // GET: /Admin/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Create

        [HttpPost]
        public ActionResult Create(RssFeedAlertSource rssfeedalertsource)
        {
            if (ModelState.IsValid)
            {
                var alertBoardUser = db.GetCurrentUser();

                var existingFeed = db.GetList<RssFeedAlertSource>(s => s.FeedUri == rssfeedalertsource.FeedUri).SingleOrDefault();

                if (existingFeed != null)
                {
                    //db.Load(existingFeed);

                    //if this user is not already subscribed to this feed
                    if (!existingFeed.UserIds.Contains(alertBoardUser.Id))
                    {                      
                        //add this user as a user of this source
                        existingFeed.UserIds.Add(alertBoardUser.Id);
                        db.StoreRssFeed(existingFeed);
                    }
                }
                else
                {
                    //TODO: Figure out how to do this!
                    if (rssfeedalertsource.UserIds == null)
                        rssfeedalertsource.UserIds = new List<string>();

                    rssfeedalertsource.UserIds.Add(alertBoardUser.Id);

                    db.StoreRssFeed(rssfeedalertsource);
                }
                
                //db.SaveChanges();
                
                return RedirectToAction("Index");  
            }

            return View(rssfeedalertsource);
        }
        
        //
        // GET: /Admin/Edit/5
 
        public ActionResult Edit(int id)
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssFeed = db.Get<RssFeedAlertSource>(id);

            return View(rssFeed);
        }

        //
        // POST: /Admin/Edit/5

        [HttpPost]
        public ActionResult Edit(RssFeedAlertSource rssfeedalertsource)
        {
            if (ModelState.IsValid)
            {
                var alertBoardUser = db.GetCurrentUser();

                if (rssfeedalertsource.UserIds.Where(u => u == alertBoardUser.Id).Count() == 0)
                    throw new ApplicationException(string.Format("Feed {0} is not subscribed to by user {1}", rssfeedalertsource.FeedUri, Membership.GetUser().UserName));

                db.StoreRssFeed(rssfeedalertsource);
                return RedirectToAction("Index");
            }
            return View(rssfeedalertsource);
        }

        //
        // GET: /Admin/Delete/5
 
        public ActionResult Delete(int id)
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssFeed = db.Get<RssFeedAlertSource>(id);

            return View(rssFeed);
        }

        //
        // POST: /Admin/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssfeedalertsource = db.Get<RssFeedAlertSource>(id);
            //db.Load(rssfeedalertsource);

            if (rssfeedalertsource.UserIds != null)
            {
                if (rssfeedalertsource.UserIds.Contains(alertBoardUser.Id))
                {
                    rssfeedalertsource.UserIds.Remove(alertBoardUser.Id);

                    //if this is the last user for this feed
                    if (rssfeedalertsource.UserIds.Count == 0)
                    {
                        //delete the feed
                        db.DeleteRssFeed(rssfeedalertsource);
                        //db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
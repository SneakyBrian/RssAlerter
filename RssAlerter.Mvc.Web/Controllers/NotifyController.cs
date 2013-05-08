using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RssAlerter.Models;
using SignalR;
using SignalR.Hosting.AspNet;
using RssAlerter.Mvc.Web.Models;
using SignalR.Infrastructure;
using Newtonsoft.Json;
using System.IO;
using RssAlerter.Mvc.Web.Models.Persistence;
using RssAlerter.Models.AlertBoards;
using RssAlerter.Mvc.Web.Models.Persistence.Extensions;
using RssAlerter.Models.AlertSources;
using RssAlerter.Mvc.Web.Extensions;

namespace RssAlerter.Mvc.Web.Controllers
{
    public class NotifyController : Controller
    {
        private RssAlerterDB db = new RssAlerterDB();

        //
        // GET: /Notify/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewItem()
        {
            var sr = new StreamReader(Request.InputStream);

            var itemString = sr.ReadToEnd();

            var item = JsonConvert.DeserializeObject<RssItem>(itemString);

            if (item != null)
            {
                IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
                dynamic clients = connectionManager.GetClients<AlertHub>();

                //foreach (var board in db.AlertBoards.Select(b => new { id = b.AlertBoardId, sources = b.RssFeedAlertSources.Select(s => s.FeedUriString) }))

//                var boards = db.Query<AlertBoard>(@"select ab.*
//                                                      from AlertBoard as ab
//                                                      join AlertBoardRssFeeds as af on ab.Id = af.AlertBoardId
//                                                      join RssFeedAlertSource as rf on af.RssFeedId = rf.Id
//                                                      where rf.FeedUriString = @FeedUriString", new { FeedUriString = item.FeedLink.ToString() }).ToList();

                using(var session = db.BeginSession())
                {
                    var feeds = db.Query<RssFeedAlertSource>(session).Where(rf => rf.FeedUri == item.FeedLink);
                    var boards = db.Query<AlertBoard>(session).Where(ab => ab.RssFeedIds.ContainsAny(feeds.Select(f => f.Id)));

                    foreach (var board in boards)
                    {
                        clients[board.Id.ToString()].notifyItem(item);

                        ItemStore.Current.Add(board, item);
                    }
                }

                return Json(new { Status = "OK" });
            }
            else
                return Json(new { Status = "Failed" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public ActionResult NewItems(IEnumerable<RssItem> items)
        {
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<AlertHub>();

            foreach (var board in db.GetList<AlertBoard>())
            {
                foreach (var sourceId in board.RssFeedIds)
                {
                    var source = db.Get<RssFeedAlertSource>(sourceId);

                    foreach (var item in items)
                    {
                        if (source.FeedUri == item.FeedLink)
                        {
                            clients[board.Id.ToString()].notifyItem(item);
                        }
                    }
                }
            }

            return View();
        }

        public ActionResult Test()
        {
            var item = new RssItem
            {
                Title = "This is a test",
                PubDate = DateTime.Now,
                Description = "This is just a test item and can safely be ignored",
                Category = "Test",
                FeedLink = new Uri("http://www.google.com"),
                Guid = Guid.NewGuid().ToString(),
                Link = new Uri("http://www.google.com")
            };

            return View(item);
        }

        [HttpPost]
        public ActionResult Test(RssItem item)
        {
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<AlertHub>();

            clients.notifyItem(item);

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

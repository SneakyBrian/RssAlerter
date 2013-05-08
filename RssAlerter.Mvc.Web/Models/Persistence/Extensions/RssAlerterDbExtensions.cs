using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssAlerter.Models.AlertBoards;
using System.Web.Security;
using DapperExtensions;
using RssAlerter.Models.AlertSources;

namespace RssAlerter.Mvc.Web.Models.Persistence.Extensions
{
    public static class RssAlerterDbExtensions
    {
        public static User GetCurrentUser(this RssAlerterDB db)
        {
            var currentUser = Membership.GetUser().ProviderUserKey.ToString();
            var alertBoardUser = db.GetList<User>(f => f.MembershipId == currentUser).Single();

            return alertBoardUser;
        }

        public static void StoreAlertBoard(this RssAlerterDB db, AlertBoard alertBoard)
        {
            using (var session = db.BeginSession())
            {
                db.Store(session, alertBoard);

                session.SaveChanges();
            }
        }

        public static void DeleteAlertBoard(this RssAlerterDB db, AlertBoard alertBoard)
        {
            using (var session = db.BeginSession())
            {
                db.Delete(session, alertBoard);

                session.SaveChanges();
            }
        }

        public static void StoreUser(this RssAlerterDB db, User user)
        {
            using (var session = db.BeginSession())
            {
                db.Store(session, user);

                session.SaveChanges();
            }
        }

        public static void DeleteUser(this RssAlerterDB db, User user)
        {
            using (var session = db.BeginSession())
            {
                db.Delete(session, user);

                session.SaveChanges();
            }
        }

        public static void StoreRssFeed(this RssAlerterDB db, RssFeedAlertSource rssFeed)
        {
            using (var session = db.BeginSession())
            {
                db.Store(session, rssFeed);

                session.SaveChanges();
            }
        }

        public static void DeleteRssFeed(this RssAlerterDB db, RssFeedAlertSource rssFeed)
        {
            using (var session = db.BeginSession())
            {
                db.Delete(session, rssFeed);

                session.SaveChanges();
            }
        }
    }
}
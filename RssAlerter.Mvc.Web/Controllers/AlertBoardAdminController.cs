using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RssAlerter.Models.AlertBoards;
using System.Web.Security;
using RssAlerter.Mvc.Web.Models;
using DapperExtensions;
using RssAlerter.Mvc.Web.Models.Persistence;
using RssAlerter.Mvc.Web.Models.Persistence.Extensions;
using RssAlerter.Models.AlertSources;

namespace RssAlerter.Mvc.Web.Controllers
{ 
    [Authorize]
    public class AlertBoardAdminController : Controller
    {
        private RssAlerterDB db = new RssAlerterDB();

        //
        // GET: /AlertBoardAdmin/

        public ViewResult Index()
        {
            var alertBoardUser = db.GetCurrentUser();

            var alertBoards = db.GetList<AlertBoard>(alertBoardUser.AlertBoardIds);

            return View(alertBoards);
        }

        //
        // GET: /AlertBoardAdmin/Details/5

        public ActionResult Details(string id)
        {
            var alertBoardUser = db.GetCurrentUser();
            var alertBoard = db.GetList<AlertBoard>(ab => ab.Id == id).Single();

            return View(alertBoard);
        }

        //
        // GET: /AlertBoardAdmin/Create

        public ActionResult Create()
        {
            var alertBoardUser = db.GetCurrentUser();

            var rssFeeds = db.GetList<RssFeedAlertSource>(alertBoardUser.RssFeedIds);

            return View(new AlertBoardViewModel(alertBoardUser, rssFeeds));
        } 

        //
        // POST: /AlertBoardAdmin/Create

        [HttpPost]
        public ActionResult Create(AlertBoardViewModel alertboardVM)
        {
            if (ModelState.IsValid)
            {
                var alertBoardUser = db.GetCurrentUser();
                var alertBoard = alertboardVM.ToAlertBoard(alertBoardUser);
                db.StoreAlertBoard(alertBoard);
                //db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(alertboardVM);
        }
        
        //
        // GET: /AlertBoardAdmin/Edit/5
 
        public ActionResult Edit(int id)
        {
            var alertboard = db.Get<AlertBoard>(id);
            //db.Load(alertboard, depth: 2);

            var alertBoardUser = db.GetCurrentUser();

            if (alertboard.UserId != alertBoardUser.Id)
                return RedirectToAction("Index");

            var rssFeeds = db.GetList<RssFeedAlertSource>(alertBoardUser.RssFeedIds);

            return View(new AlertBoardViewModel(alertboard, rssFeeds));
        }

        //
        // POST: /AlertBoardAdmin/Edit/5

        [HttpPost]
        public ActionResult Edit(AlertBoardViewModel alertboardVM)
        {
            if (ModelState.IsValid)
            {
                var alertBoardUser = db.GetCurrentUser();

                //var alertBoard = alertBoardUser.AlertBoard.Single(b => b.Id == alertboardVM.AlertBoardId);
                var alertBoard = db.Get<AlertBoard>(alertboardVM.AlertBoardId);

                db.StoreAlertBoard(alertboardVM.SetAlertBoard(alertBoard));                
                
                return RedirectToAction("Index");
            }
            return View(alertboardVM);
        }

        //
        // GET: /AlertBoardAdmin/Delete/5
 
        public ActionResult Delete(int id)
        {
            var alertboard = db.Get<AlertBoard>(id);

            var alertBoardUser = db.GetCurrentUser();

            if (alertboard.UserId != alertBoardUser.Id)
                return RedirectToAction("Index"); 
            
            return View(alertboard);
        }

        //
        // POST: /AlertBoardAdmin/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var alertboard = db.Get<AlertBoard>(id);

            var alertBoardUser = db.GetCurrentUser();

            if (alertboard.UserId != alertBoardUser.Id)
                return RedirectToAction("Index"); 
            
            db.DeleteAlertBoard(alertboard);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
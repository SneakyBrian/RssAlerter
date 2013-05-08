using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using System.Collections;
using RssAlerter.Models;
using System.Diagnostics;
using RssAlerter.Mvc.Web.Models.Persistence;
using RssAlerter.Models.AlertBoards;

namespace RssAlerter.Mvc.Web.Models
{
    public class AlertHub : Hub, IDisposable
    {
        private RssAlerterDB db = new RssAlerterDB();

        public void Register(int boardID)
        {
            Debug.WriteLine("got register call from client");

            AddToGroup(boardID.ToString());

            var board = db.Get<AlertBoard>(boardID);

            foreach (var item in ItemStore.Current.Get(board))
            {
                this.Clients[Context.ConnectionId].notifyItem(item);    
            }            
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
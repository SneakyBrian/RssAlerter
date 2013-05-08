using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssAlerter.Models.AlertBoards;
using RssAlerter.Models;
using System.Web.Caching;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using Raven.Client;
using Microsoft.Practices.ServiceLocation;

namespace RssAlerter.Mvc.Web.Models.Persistence
{
    public class ItemStore
    {
        private const string CACHE_KEY_FORMAT = "Board{0}_Item{1}_{2}";

        private readonly HashAlgorithm hashAlgorithm;
        private readonly IDocumentStore documentStore;

        private class BoardItem
        {
            public AlertBoard AlertBoard { get; set; }
            public RssItem RssItem { get; set; }
            public bool Approved { get; set; }
        }

        public static ItemStore Current { get; private set; }

        private ItemStore()
        {
            hashAlgorithm = new MD5CryptoServiceProvider();
            documentStore = ServiceLocator.Current.GetInstance<IDocumentStore>();
        }

        static ItemStore()
        {
            Current = new ItemStore();
        }

        public void Add(AlertBoard board, RssItem item)
        {
            var id = GetCacheKey(board, item);

            using (var session = documentStore.OpenSession())
            {
                if (session.Load<BoardItem>(id) == null)
                {
                    session.Store(new BoardItem { AlertBoard = board, RssItem = item }, id);
                    session.SaveChanges();
                }
            }
        }

        //public void SetApproval(AlertBoard board, RssItem item, bool approved)
        //{
        //    using (var session = documentStore.OpenSession())
        //    {
        //        var boardItem = session.Load<BoardItem>(GetCacheKey(board, item));
        //        boardItem.Approved = approved;

        //        session.Store(boardItem);
        //        session.SaveChanges();
        //    }
        //}

        public IEnumerable<RssItem> Get(AlertBoard board)
        {
            return Get(board, TimeSpan.FromDays(7));
        }

        public IEnumerable<RssItem> Get(AlertBoard board, TimeSpan maxAge)
        {
            return Get(board, DateTime.UtcNow.Subtract(maxAge));
        }

        public IEnumerable<RssItem> Get(AlertBoard board, DateTime newerThan)
        {
            List<RssItem> results;

            using (var session = documentStore.OpenSession())
            {
                results = session.Query<BoardItem>()
                    .Where(item => item.AlertBoard.Id == board.Id)
                    .Where(item => item.RssItem.PubDate > newerThan)
                    .Select(item => item.RssItem)
                    .ToList();
            }

            return results;
        }

        private string GetCacheKey(AlertBoard board, RssItem item)
        {
            var guid = new Guid(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(item.Guid)));
            var link = new Guid(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(item.Link.ToString())));

            return string.Format(CACHE_KEY_FORMAT, board.Id, guid, link);
        }
    }
}
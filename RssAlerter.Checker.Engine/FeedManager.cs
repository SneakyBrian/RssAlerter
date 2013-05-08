using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Contexts;

namespace RssAlerter.Checker.Engine
{
    public class FeedManager : IDisposable
    {
        private readonly List<FeedChecker> _feeds;
        private readonly Context _context;

        public event EventHandler<NewRssItemEventArgs> NewFeedItem;

        public FeedManager()
        {
            _context = Thread.CurrentContext;
            _feeds = new List<FeedChecker>();
        }

        public void AddFeed(Uri feedUri, TimeSpan updateFrequency)
        {
            //if this feed is already in the list, return
            if (_feeds.Count(f => f.FeedUri == feedUri) > 0)
                return;

            var feed = new FeedChecker(feedUri, updateFrequency);

            feed.NewRssItem += feed_NewRssItem;

            _feeds.Add(feed);
        }

        void feed_NewRssItem(object sender, NewRssItemEventArgs e)
        {
            //make sure we raise this event on the thread this was created on
            _context.DoCallBack(() =>
            {
                var handler = NewFeedItem;
                if (handler != null)
                    handler(this, e);
            });
        }

        public void Dispose()
        {
            foreach (var feed in _feeds)
            {
                feed.NewRssItem -= feed_NewRssItem;
                feed.Dispose();
            }

            _feeds.Clear();
        }

        public void RemoveFeed(Uri feed)
        {
            var fc = _feeds.SingleOrDefault(f => f.FeedUri == feed);

            if (fc != null)
            {
                fc.NewRssItem -= feed_NewRssItem;
                fc.Dispose();
            }

            _feeds.Remove(fc);
        }
    }
}

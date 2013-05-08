using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Xml.Linq;
using RssAlerter.Models;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace RssAlerter.Checker.Engine
{
    public class FeedChecker : IDisposable
    {
        private readonly Context _context;
        private readonly WebClient _webClient;

        private DateTime _lastUpdate;

        private CancellationTokenSource _cancellationTokenSource;

        public Uri FeedUri { get; private set; }

        public TimeSpan UpdateFrequency { get; set; }

        public event EventHandler<NewRssItemEventArgs> NewRssItem;

        public FeedChecker(Uri feedUri, TimeSpan updateFrequency)
        {
            FeedUri = feedUri;
            UpdateFrequency = updateFrequency;

            _context = Thread.CurrentContext;

            _lastUpdate = DateTime.MinValue;

            _cancellationTokenSource = new CancellationTokenSource();

            _webClient = new WebClient();

            RecursivelyCheckFeed();
        }


        private void RecursivelyCheckFeed()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                Task.Factory.StartNew(CheckFeed, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith(t => RecursivelyCheckFeed());
            }
        }

        private void CheckFeed() 
        {
            try
            {
                var feedContents = _webClient.DownloadString(FeedUri);

                var document = XDocument.Parse(feedContents);

                //convert feed into list of items
                var rssItems = document.Descendants("item").Select(n => new RssItem
                {
                    Title = n.Element("title") != null ? n.Element("title").Value : string.Empty,
                    Category = n.Element("category") != null ? n.Element("category").Value : string.Empty,
                    Description = n.Element("description") != null ? n.Element("description").Value : string.Empty,
                    FeedLink = FeedUri,
                    Guid = n.Element("guid") != null ? n.Element("guid").Value : string.Empty,
                    Link = n.Element("link") != null ? new Uri(n.Element("link").Value) : null,
                    PubDate = n.Element("pubDate") != null ? DateTime.Parse(n.Element("pubDate").Value) : DateTime.UtcNow
                }).Where(i => i.PubDate > _lastUpdate).OrderBy(i => i.PubDate).ToList();

                foreach (var rssItem in rssItems)
                {
                    //make sure we raise this event on the thread this was created on
                    _context.DoCallBack(() =>
                    {
                        var handler = NewRssItem;
                        if (handler != null)
                            handler(this, new NewRssItemEventArgs(rssItem));
                    });

                    if (rssItem.PubDate > _lastUpdate)
                        _lastUpdate = rssItem.PubDate;
                }
            }
            catch { }
            finally
            {
                //wait for the update frequency time
                if (!_cancellationTokenSource.IsCancellationRequested)
                    Thread.Sleep(UpdateFrequency);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }


    public class NewRssItemEventArgs : EventArgs
    {
        public RssItem Item { get; private set; }

        public NewRssItemEventArgs(RssItem item)
        {
            Item = item;
        }
    }
}

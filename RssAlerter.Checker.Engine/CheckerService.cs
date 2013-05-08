using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using RssAlerter.Models;

namespace RssAlerter.Checker.Engine
{
    public class CheckerService : IDisposable
    {
        private Uri _feedListUri;
        private Uri _postItemUri;
        private TimeSpan _updateFrequency;
        private WebClient _webClient;
        private FeedManager _feedManager;
        private IEnumerable<Uri> _feedList;
        private Timer _refreshTimer;

        public event EventHandler<NewItemSubmittedEventArgs> NewItemSubmitted;
        public event EventHandler RefreshingFeeds;

        public CheckerService(Uri feedListUri, Uri postItemUri, TimeSpan updateFrequency)
        {
            _feedListUri = feedListUri;
            _postItemUri = postItemUri;
            _updateFrequency = updateFrequency;

            _webClient = new WebClient();

            _refreshTimer = new Timer(new TimerCallback(obj => RefreshFeedList()), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            _feedManager = new FeedManager();
            
            _feedManager.NewFeedItem += (s,e) =>
            {
                try
                {
                    var postString = JsonConvert.SerializeObject(e.Item);

                    //when we get a new item, just submit the post for the user asynchronously...
                    var result = _webClient.UploadString(postItemUri, postString);

                    var newItem = NewItemSubmitted;
                    if (newItem != null)
                        newItem(this, new NewItemSubmittedEventArgs(e.Item, result));
                }
                catch { }
            };
        
        }

        public void Start()
        {
            //start the refresh timer
            _refreshTimer.Change(TimeSpan.Zero, this._updateFrequency);
        }

        public void Stop()
        {
            //stop the refresh timer
            _refreshTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            foreach (var feed in _feedList)
            {
                _feedManager.RemoveFeed(feed);
            }           
        }

        private void RefreshFeedList()
        {
            var refreshingFeeds = RefreshingFeeds;
            if (refreshingFeeds != null)
                refreshingFeeds(this, EventArgs.Empty);

            _feedList = OpmlLoader.GetFeedUris(this._feedListUri);

            foreach (var feed in _feedList)
            {
                _feedManager.AddFeed(feed, this._updateFrequency);
            } 
        }

        public void Dispose()
        {
            _refreshTimer.Dispose();
            _feedManager.Dispose();
        }
    }

    public class NewItemSubmittedEventArgs : EventArgs
    {
        public RssItem Item { get; private set; }
        public string Result { get; private set; }

        public NewItemSubmittedEventArgs(RssItem item, string result)
        {
            Item = item;
            Result = result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RssAlerter.Models
{
    public class RssItem
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public Uri FeedLink { get; set; }
        public string Guid { get; set; }
        public string Category { get; set; }
        public DateTime PubDate { get; set; }
        public string Description { get; set; }
    }
}

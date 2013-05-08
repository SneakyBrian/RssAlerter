using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RssAlerter.Models.AlertBoards;

namespace RssAlerter.Models.AlertSources
{
    public class RssFeedAlertSource
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public Uri FeedUri { get; set; }

        [Required]
        public TimeSpan UpdateFrequency { get; set; }

        public IList<string> UserIds { get; set; }

        public RssFeedAlertSource()
        {
            UserIds = new List<string>();
            Id = "alertsource/rssfeed/"; // db assigns id
        }
    }
}

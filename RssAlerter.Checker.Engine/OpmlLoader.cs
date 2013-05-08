using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json;

namespace RssAlerter.Checker.Engine
{
    public static class OpmlLoader
    {
        public static IEnumerable<Uri> GetFeedUris(Uri opmlUri)
        {
            var webClient = new WebClient();

            var opmlString = webClient.DownloadString(opmlUri);

            return GetFeedUris(opmlString);
        }

        public static IEnumerable<Uri> GetFeedUris(string opmlString)
        {
            var feedList = JsonConvert.DeserializeObject<Uri[]>(opmlString);

            return feedList;
        }
    }
}

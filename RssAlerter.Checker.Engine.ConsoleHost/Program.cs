using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace RssAlerter.Checker.Engine.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var feedListUri = new Uri(args[0]);
            var postItemUri = new Uri(args[1]);
            var updateFrequency = TimeSpan.Parse(args[2]);

            var quitSignal = new AutoResetEvent(false);

            Console.CancelKeyPress += (s, e) => quitSignal.Set();

            using (var checkerService = new CheckerService(feedListUri, postItemUri, updateFrequency))
            {
                checkerService.RefreshingFeeds += (s, e) => Console.WriteLine("Refreshing feeds...");
                checkerService.NewItemSubmitted += (s, e) => Console.WriteLine("Item {0} Result {1}", e.Item.Link, e.Result);

                checkerService.Start();

                quitSignal.WaitOne();

                checkerService.Stop();
            }
        }
    }
}

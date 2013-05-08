using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Embedded;

namespace RssAlerter.Mvc.Web.Models.Persistence
{
    //http://msdn.microsoft.com/pt-br/magazine/hh547101%28en-us%29.aspx

    public class DataDocumentStore
    {
        private static IDocumentStore instance;

        public static IDocumentStore Instance
        {
            get
            {
                if (instance == null)
                    throw new InvalidOperationException(
                      "IDocumentStore has not been initialized.");
                return instance;
            }
        }

        public static IDocumentStore Initialize()
        {
            instance = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" };
            instance.Conventions.IdentityPartsSeparator = "-";
            instance.Initialize();
            return instance;
        }
    }
}
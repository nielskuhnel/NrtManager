using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib.Management.Client;
using Lucene.Net.Store;

namespace LuceneManager
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static SearcherContext Searcher { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            Searcher = new SearcherContext(FSDirectory.Open(HostingEnvironment.MapPath("~/App_Data/Index")),
                new WhitespaceAnalyzer());
        }

        protected void Application_End()
        {
            Searcher.Dispose();
        }
    }
}
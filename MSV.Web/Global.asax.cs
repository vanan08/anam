using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Accuracy.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterMiniProfiler();
        }

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        private void RegisterMiniProfiler()
        {
            GlobalFilters.Filters.Add(new StackExchange.Profiling.Mvc.ProfilingActionFilter());
            var ignored = MiniProfiler.Settings.IgnoredPaths.ToList();
            ignored.Add("/__browserLink/");
            MiniProfiler.Settings.IgnoredPaths = ignored.ToArray();
        }
    }
}

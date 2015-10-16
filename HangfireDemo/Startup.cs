using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hangfire;
using Hangfire.SqlServer;
using HangfireDemo.Author;
using Owin;

namespace HangfireDemo
{
    public class Startup
    {

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HighlighterDb");
            appBuilder.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            appBuilder.UseHangfireDashboard();
            appBuilder.UseHangfireServer();
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);
        }
        //写入作业
        public void Test()
        {
            //------
        }
    }
}

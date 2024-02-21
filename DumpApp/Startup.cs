using Hangfire;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(DumpApp.Startup))]
namespace DumpApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnectionProc");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Configure other middleware, e.g., authentication, as needed
        }
    }
}
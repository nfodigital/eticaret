using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace B2B
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 0;
            var _cookie = new HttpCookie("usersid");
            _cookie.Expires = DateTime.Now.AddDays(2);
            _cookie["usersid"] = Session.SessionID;
            Response.Cookies.Add(_cookie);
        }
        void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app.Request.Path.IndexOf("tarsus-cicek-siparisi") > 0)
            {
                app.Context.RewritePath("/Home/Index");
            }
            if (app.Request.Path.IndexOf("mersin-cicek-siparisi") > 0)
            {
                app.Context.RewritePath("/Home/Index");
            }
            if (app.Request.Path.IndexOf("cicek-gider") > 0)
            {
                app.Context.RewritePath("/Home/Index");
            }

            if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Host = "www." + Request.Url.Host;
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
        }
        
    }
}
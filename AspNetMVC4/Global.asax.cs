using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AspNetMVC4
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

            //get the useragent for the request
            string currentUserAgent = HttpContext.Current.Request.UserAgent;

            //decide if we need to strip off the same site attribute for older browsers
            bool dissallowSameSiteFlag = false; //DisallowsSameSiteNone(currentUserAgent);

            //get the name of the cookie, if not defined default to the "ASP.NET_SessionID" value
            SessionStateSection sessionStateSection = (SessionStateSection)ConfigurationManager
                                                        .GetSection("system.web/sessionState");
            string sessionCookieName;
            if (sessionStateSection != null)
            {
                //read the name from the configuration
                sessionCookieName = sessionStateSection.CookieName;
            }
            else
            {
                sessionCookieName = "ASP.NET_SessionId";
            }


            //should the flag be positioned to true, then remove the attribute by setting
            //value to SameSiteMode.None
            if (dissallowSameSiteFlag)
                Response.Cookies[sessionCookieName].SameSite = (SameSiteMode)(-1);
            else
                Response.Cookies[sessionCookieName].SameSite = SameSiteMode.None;

            //while we're at it lets also make it secure
            if (Request.IsSecureConnection)
                Response.Cookies[sessionCookieName].Secure = true;

        }
    }
}

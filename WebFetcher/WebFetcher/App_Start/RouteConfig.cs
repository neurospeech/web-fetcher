using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebFetcher
{
    public class RouteConfig
    {

        public static Regex AllowedHosts;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var h = System.Web.Configuration.WebConfigurationManager.AppSettings["AllowedHosts"];

            var matches = string.Join("|", h.Split(',', ';', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().Replace("*", ".*?")));

            AllowedHosts = new Regex(matches , RegexOptions.Compiled | RegexOptions.IgnoreCase);

            routes.MapRoute(
                name: "Default",
                url: "{*all}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

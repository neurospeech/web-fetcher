using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebFetcher
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterFilters(GlobalFilters.Filters);
        }

        #region void  MvcApplication_Error(object sender, EventArgs e)
        void MvcApplication_Error(object sender, EventArgs e)
        {
            Response.Filter = null;
        }
        #endregion

    }
}

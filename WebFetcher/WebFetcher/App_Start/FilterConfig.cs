using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebFetcher
{
    public class FilterConfig
    {


        internal static void RegisterFilters(System.Web.Mvc.GlobalFilterCollection filters)
        {
            filters.Add(new CompressionFilterAttribute());
        }
    }

    public class CompressionFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;

            //if(context.Request.RawUrl)
            string mime = MimeMapping.GetMimeMapping(context.Request.RawUrl) ?? "";
            if (!mime.Equals("application/json", StringComparison.OrdinalIgnoreCase))
            {
                if (!mime.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                    return;
            }
            var ae = context.Request.Headers["Accept-Encoding"];
            if (ae != null)
            {
                ae = ae.ToLower();
                var response = context.Response;
                if (ae.Contains("gzip"))
                {
                    response.AddHeader("Content-Encoding", "gzip");
                    response.Filter =
                        new System.IO.Compression.GZipStream(response.Filter, System.IO.Compression.CompressionMode.Compress);
                }
                else if (ae.Contains("deflate"))
                {
                    response.AddHeader("Content-Encoding", "deflate");
                    response.Filter =
                        new System.IO.Compression.DeflateStream(response.Filter, System.IO.Compression.CompressionMode.Compress);
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }
}
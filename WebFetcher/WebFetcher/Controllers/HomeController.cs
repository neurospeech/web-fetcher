using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebFetcher.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public async Task<ActionResult> Index(string all)
        {

            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            if (string.Equals(Request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase)) {
                Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type, Origin");
                Response.AppendHeader("Access-Control-Max-Age", "1728000");
            }

            using (HttpClient client = new HttpClient())
            {

                bool isSecure = Request.IsSecureConnection;

                UriBuilder builder = new UriBuilder("http:/" + Request.RawUrl);

                if(!RouteConfig.AllowedHosts.IsMatch(builder.Host)){
                    throw new InvalidOperationException(builder.Host + " not allowed.");
                }

                //if(isSecure){
                //    builder.Scheme = "https";
                //}

                var r = await client.GetAsync(builder.ToString());

                return await HttpResponseActionResult.New(r);
            }
        }

        private void SetCache(HttpCachePolicyBase cache, System.Net.Http.Headers.CacheControlHeaderValue cacheIn)
        {
            if (cacheIn.Public) {
                cache.SetCacheability(HttpCacheability.Public);
            }
            if (cacheIn.Private) {
                cache.SetCacheability(HttpCacheability.Private);
            }
            if (cacheIn.MaxAge != null) {
                cache.SetMaxAge(cacheIn.MaxAge.Value);
            }
            
        }
    }

    public class HttpResponseActionResult : ActionResult
    {

        public static async Task<HttpResponseActionResult> New(HttpResponseMessage msg)
        {
            var s = await msg.Content.ReadAsByteArrayAsync();
            return new HttpResponseActionResult { 
                Content=s, 
                ResponseMessage = msg 
            };
        } 

        public HttpResponseMessage ResponseMessage { get; set; }

        public byte[] Content { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            Response.StatusCode = (int)ResponseMessage.StatusCode;
            Response.StatusDescription = ResponseMessage.ReasonPhrase;
            Response.TrySkipIisCustomErrors = true;

            if (Response.StatusCode == 200)
            {
                SetCache(Response.Cache, ResponseMessage.Headers.CacheControl);
            }
            else
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }

            var content = ResponseMessage.Content;
            var val = content.Headers.Expires;
            if (val != null)
            {
                Response.ExpiresAbsolute = val.Value.UtcDateTime;
            }

            if (content.Headers.ContentType != null)
            {
                Response.ContentType = content.Headers.ContentType.ToString();
            }

            if (ResponseMessage.Headers.Location != null)
            {
                Response.RedirectLocation = ResponseMessage.Headers.Location.ToString();
            }
            else
            {
                if (Content != null)
                {
                    Response.OutputStream.Write(Content, 0, Content.Length);
                }
            }
        }

        private void SetCache(HttpCachePolicyBase cache, System.Net.Http.Headers.CacheControlHeaderValue cacheIn)
        {
            if (cacheIn.Public)
            {
                cache.SetCacheability(HttpCacheability.Public);
            }
            if (cacheIn.Private)
            {
                cache.SetCacheability(HttpCacheability.Private);
            }
            if (cacheIn.MaxAge != null)
            {
                var maxAge = cacheIn.MaxAge.Value;
                if (maxAge.TotalDays < 1) {
                    maxAge = TimeSpan.FromDays(30);
                }
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetMaxAge(maxAge);
                cache.SetSlidingExpiration(true);
            }

        }
    }
}
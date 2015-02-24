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
                if (r.StatusCode == System.Net.HttpStatusCode.OK) {


                    var s = await r.Content.ReadAsByteArrayAsync();
                    //Response.CacheControl = r.Headers.CacheControl.


                    var val = r.Content.Headers.Expires;
                    if (val != null)
                    {
                        Response.ExpiresAbsolute = val.Value.UtcDateTime;
                    }
                    else {
                        Response.CacheControl = "public,max-age=3240000";
                    }

                    return this.File(s, r.Content.Headers.ContentType.ToString());

                }

                using (StringWriter sw = new StringWriter()) {
                    sw.WriteLine("Error retreiving " + builder.ToString());
                    sw.WriteLine("Http Status Code: " + r.StatusCode.ToString());

                    var str = await r.Content.ReadAsStringAsync();

                    // this response must not be cached anywhere ...
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    // this must expire immediately ...
                    Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(-2);


                    return new HttpStatusCodeResult(r.StatusCode, str);
                }
            }
        }
    }
}
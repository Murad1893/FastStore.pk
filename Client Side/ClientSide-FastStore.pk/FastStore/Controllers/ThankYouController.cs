using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FastStore.Models;
using Newtonsoft.Json;

namespace FastStore.Controllers
{
    public class ThankYouController : Controller
    {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        // GET: ThankYou
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "api/product/categories";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

                ViewBag.cartBox = null;
                ViewBag.Total = null;
                ViewBag.NoOfItem = null;
                TempShpData.items = null;
                return View("Thankyou");
            }
                
        }
    }
}
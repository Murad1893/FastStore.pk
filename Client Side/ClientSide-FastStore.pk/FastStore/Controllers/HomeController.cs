using FastStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace FastStore.Controllers
{
    public class HomeController : Controller
    {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        FastStoreEntities db = new FastStoreEntities();

        // GET: Home
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/product/category";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["categoryName"] = "Men";
                builder.Query = query.ToString();
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.MenProduct = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

                builder.Path = "/api/product/category";
                query = HttpUtility.ParseQueryString(builder.Query);
                query["categoryName"] = "Women";
                builder.Query = query.ToString();
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();
                ViewBag.WomenProduct = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

                builder.Path = "/api/product/category";
                query = HttpUtility.ParseQueryString(builder.Query);
                query["categoryName"] = "Sports";
                builder.Query = query.ToString();
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();
                ViewBag.SportsProduct = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

                builder.Path = "/api/product/category";
                query = HttpUtility.ParseQueryString(builder.Query);
                query["categoryName"] = "Phones";
                builder.Query = query.ToString();
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();
                ViewBag.ElectronicsProduct = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

                builder.Path = "/api/genMainSliders";
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();
                ViewBag.Slider = JsonConvert.DeserializeObject<IEnumerable<genMainSlider>>(content);
                
                ViewBag.PromoRight = db.genPromoRights.ToList();

                this.GetDefaultData();
                return View();
            }

        }      

    }
}
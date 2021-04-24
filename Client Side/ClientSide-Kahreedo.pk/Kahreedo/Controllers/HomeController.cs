using Khareedo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Khareedo.Controllers
{
    public class HomeController : Controller
    {
        KhareedoEntities db = new KhareedoEntities();

        // GET: Home
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                ViewBag.MenProduct = db.Products.Where(x => x.Category.Name.Equals("Men")).ToList();
                ViewBag.WomenProduct = db.Products.Where(x => x.Category.Name.Equals("Women")).ToList();
                ViewBag.SportsProduct = db.Products.Where(x => x.Category.Name.Equals("Sports")).ToList();
                ViewBag.ElectronicsProduct = db.Products.Where(x => x.Category.Name.Equals("Phones")).ToList();

                var response = await client.GetAsync("http://localhost:2509/api/genMainSliders");
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.Slider = JsonConvert.DeserializeObject<IEnumerable<genMainSlider>>(content);
                
                ViewBag.PromoRight = db.genPromoRights.ToList();

                this.GetDefaultData();
                return View();
            }

        }      

    }
}
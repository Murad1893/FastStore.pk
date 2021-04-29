using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FastStore.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;

namespace FastStore.Controllers
{
    public class MyCartController : Controller
    {
        FastStoreEntities db = new FastStoreEntities();
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        // GET: MyCart
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                var data = await this.GetDefaultData();

                builder.Path = "api/product/categories";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);
                return View(data);
            }
        }

        public ActionResult Remove(int id)
        {
            TempShpData.items.RemoveAll(x=>x.ProductID==id);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult ProcedToCheckout(FormCollection formcoll)
        {
            var a = TempShpData.items.ToList();
            for (int i = 0; i < formcoll.Count/2; i++)
            {

                int pID = Convert.ToInt32(formcoll["shcartID-" + i + ""]);
                var ODetails = TempShpData.items.FirstOrDefault(x => x.ProductID == pID);
               

                int qty = Convert.ToInt32(formcoll["Qty-" + i + ""]);
                ODetails.Quantity = qty;
                ODetails.UnitPrice = ODetails.UnitPrice;
                ODetails.TotalAmount = qty * ODetails.UnitPrice;
                TempShpData.items.RemoveAll(x => x.ProductID == pID);

                if (TempShpData.items==null)
                {
                    TempShpData.items = new List<OrderDetail>();                   
                }
                TempShpData.items.Add(ODetails);
                
            }

            return RedirectToAction("Index", "CheckOut");
        }
        
        
    }
}
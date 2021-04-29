using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using FastStore.Models;
using Newtonsoft.Json;

namespace FastStore.Controllers
{
    public class WishListController : Controller
    {
        FastStoreEntities db = new FastStoreEntities();
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);

        // GET: WishList
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "api/product/categories";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

                builder.Path = "/api/wishlist";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["customerId"] = TempShpData.UserID.ToString();
                builder.Query = query.ToString();
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();

                await this.GetDefaultData();

                return View(JsonConvert.DeserializeObject<IEnumerable<Wishlist>>(content).ToList());
            }
        }

        //REMOVE ITEM FROM WISHLIST
        public async Task<ActionResult> Remove(int id)
        {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/wishlist";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["id"] = id.ToString();
                builder.Query = query.ToString();

                var response = await client.DeleteAsync(builder.Uri);
            }

            return RedirectToAction("Index");

        }
        //ADD TO CART WISHLIST
        public async Task<ActionResult> AddToCart(int id)
        {
            using (var client = new HttpClient())
            {
                OrderDetail OD = new OrderDetail();

                builder.Path = "/api/wishlist";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["id"] = id.ToString();
                builder.Query = query.ToString();

                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                Wishlist w = JsonConvert.DeserializeObject<Wishlist>(content);

                int pid = w.ProductID;
                OD.ProductID = pid;
                int Qty = 1;

                decimal price = w.Product.UnitPrice;
                OD.Quantity = Qty;
                OD.UnitPrice = price;
                OD.TotalAmount = Qty * price;
                OD.Product = w.Product;

                if (TempShpData.items == null)
                {
                    TempShpData.items = new List<OrderDetail>();
                }
                TempShpData.items.Add(OD);

                await Remove(id);

                return Redirect(TempData["returnURL"].ToString());
            }
        }
    }
}
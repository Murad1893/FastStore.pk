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
    public static class LoadDataController 
    {
        static FastStoreEntities db = new FastStoreEntities();
        static UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        public static async Task<List<OrderDetail>> GetDefaultData(this ControllerBase controller)
        {
            using (var client = new HttpClient())
            {
                if (TempShpData.items == null)
                {
                    TempShpData.items = new List<OrderDetail>();
                }
                var data = TempShpData.items.ToList();

                controller.ViewBag.cartBox = data.Count == 0 ? null : data;
                controller.ViewBag.NoOfItem = data.Count();
                int? SubTotal = Convert.ToInt32(data.Sum(x => x.TotalAmount));
                controller.ViewBag.Total = SubTotal;

                int Discount = 0;
                controller.ViewBag.SubTotal = SubTotal;
                controller.ViewBag.Discount = Discount;
                controller.ViewBag.TotalAmount = SubTotal - Discount;

                builder.Path = "/api/wishlist";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["customerId"] = TempShpData.UserID.ToString();
                builder.Query = query.ToString();
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                controller.ViewBag.WlItemsNo = JsonConvert.DeserializeObject<IEnumerable<Wishlist>>(content).ToList().Count();
                return data;
            }
            
        }
    }
}
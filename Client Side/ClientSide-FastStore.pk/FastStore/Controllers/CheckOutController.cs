using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastStore.Models;
using System.Data;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;

namespace FastStore.Controllers
{
    public class CheckOutController : Controller
    {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        FastStoreEntities db = new FastStoreEntities();
        // GET: CheckOut
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "api/product/categories";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

                builder.Path = "/api/checkout/paymentTypes";
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();

                ViewBag.PayMethod = new SelectList(JsonConvert.DeserializeObject<IEnumerable<PaymentType>>(content), "PayTypeID", "TypeName");

                var data = await this.GetDefaultData();


                return View(data);
            }
                
        }

        
        //PLACE ORDER--LAST STEP
        public async Task<ActionResult> PlaceOrder(FormCollection getCheckoutDetails)
        {
            using (var client = new HttpClient())
            {
                int shpID = 1;
                builder.Path = "/api/checkout/nextShippingID";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) { 
                    shpID = JsonConvert.DeserializeObject<int>(content);
                }

                int payID = 1;
                builder.Path = "/api/checkout/nextPaymentID";
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    payID = JsonConvert.DeserializeObject<int>(content);
                }

                int orderID = 1;
                builder.Path = "/api/checkout/nextOrderID";
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    orderID = JsonConvert.DeserializeObject<int>(content);
                }

                ShippingDetail shpDetails = new ShippingDetail();
                shpDetails.ShippingID = shpID;
                shpDetails.FirstName = getCheckoutDetails["FirstName"];
                shpDetails.LastName = getCheckoutDetails["LastName"];
                shpDetails.Email = getCheckoutDetails["Email"];
                shpDetails.Mobile = getCheckoutDetails["Mobile"];
                shpDetails.Address = getCheckoutDetails["Address"];
                shpDetails.Province = getCheckoutDetails["Province"];
                shpDetails.City = getCheckoutDetails["City"];
                shpDetails.PostCode = getCheckoutDetails["PostCode"];

                builder.Path = "/api/checkout/shippingDetails";
                var postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(shpDetails), Encoding.UTF8, "application/json"));

                Payment pay = new Payment();
                pay.PaymentID = payID;
                pay.Type = Convert.ToInt32(getCheckoutDetails["PayMethod"]);

                builder.Path = "/api/checkout/payments";
                postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(pay), Encoding.UTF8, "application/json"));
                
                Order o = new Order();
                o.OrderID = orderID;
                o.CustomerID = TempShpData.UserID;
                o.PaymentID = payID;
                o.ShippingID = shpID;
                o.Discount = Convert.ToInt32(getCheckoutDetails["discount"]);
                o.TotalAmount = Convert.ToInt32(getCheckoutDetails["totalAmount"]);
                o.isCompleted = true;
                o.OrderDate = DateTime.Now;

                builder.Path = "/api/checkout/orders";
                postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json"));

                foreach (var OD in TempShpData.items)
                {
                    OD.OrderID = orderID;

                    builder.Path = "/api/orders/";
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["orderID"] = orderID.ToString();
                    builder.Query = query.ToString();
                    response = await client.GetAsync(builder.Uri);
                    content = await response.Content.ReadAsStringAsync();

                    OD.Order = JsonConvert.DeserializeObject<Order>(content);

                    builder.Path = "/api/product/";
                    query = HttpUtility.ParseQueryString(builder.Query);
                    query["id"] = OD.ProductID.ToString();
                    builder.Query = query.ToString();
                    response = await client.GetAsync(builder.Uri);
                    content = await response.Content.ReadAsStringAsync();

                    OD.Product = JsonConvert.DeserializeObject<Product>(content);

                    builder.Path = "/api/checkout/orderDetails";
                    postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(OD), Encoding.UTF8, "application/json"));
                }


                return RedirectToAction("Index", "ThankYou");
            }
        }

    }
}
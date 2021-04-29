using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastStore.Models;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace FastStore.Controllers
{
    public class AccountController : Controller
    {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        FastStoreEntities db = new FastStoreEntities();

        // GET: Account
        public async Task<ActionResult> Index()
        {
            await this.GetDefaultData();

            using (var client = new HttpClient())
            {

                builder.Path = "/api/customers/";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["id"] = TempShpData.UserID.ToString();
                builder.Query = query.ToString();
                var response  = await client.GetAsync(builder.Uri);
                string content  = await response.Content.ReadAsStringAsync();

                var usr = JsonConvert.DeserializeObject<Customer>(content);

                builder.Path = "api/product/categories";
                response = await client.GetAsync(builder.Uri);
                content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

                return View(usr);
            }
        }


        //REGISTER CUSTOMER
        [HttpPost]
        public async Task<ActionResult> Register(Customer cust)
        {
            using (var client = new HttpClient())
            {
                if (ModelState.IsValid)
                {
                    builder.Path = "/api/customers";
                    var postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(cust), Encoding.UTF8, "application/json"));
                    
                    Session["username"] = cust.UserName;
                    Customer customerEntity = await GetUser(cust.UserName);
                    TempShpData.UserID = customerEntity.CustomerID;
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
                
        }

        //LOG IN
        public async Task<ActionResult> Login()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "api/product/categories";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Login(FormCollection formColl)
        {
            using (var client = new HttpClient())
            {
                string usrName = formColl["UserName"];
                string Pass = formColl["Password"];

                if (ModelState.IsValid)
                {
                    builder.Path = "/api/customers/";
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["username"] = usrName;
                    query["password"] = Pass;
                    builder.Query = query.ToString();
                    var response = await client.GetAsync(builder.Uri);
                    string content = await response.Content.ReadAsStringAsync();

                    var cust = JsonConvert.DeserializeObject<Customer>(content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempShpData.UserID = cust.CustomerID;
                        Session["username"] = cust.UserName;
                        return RedirectToAction("Index", "Home");
                    }

                }


                return View();
            }
        }

        //LOG OUT
         public ActionResult Logout()
         {
             Session["username"] = null;
             TempShpData.UserID = 0;
             TempShpData.items = null;
             return RedirectToAction("Index", "Home");
         }

       

        public async Task<Customer> GetUser(string _usrName)
        {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/customers/";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["username"] = _usrName;
                builder.Query = query.ToString();
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                var cust = JsonConvert.DeserializeObject<Customer>(content);

                return cust;
            }
        }

        //UPDATE CUSTOMER DATA
        [HttpPost]
        public async Task<ActionResult> Update(Customer cust)
        {
            using (var client = new HttpClient())
            {
                if (ModelState.IsValid)
                {
                    builder.Path = "/api/customers/";
                    var putTask = await client.PutAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(cust), Encoding.UTF8, "application/json"));

                    Session["username"] = cust.UserName;

                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
        }
    }
}
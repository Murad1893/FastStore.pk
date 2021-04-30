using FastStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FastStore.Controllers
{
    public class ProductController : Controller
    {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        FastStoreEntities db = new FastStoreEntities();

        
        public async Task<ActionResult> Index()
        {
            GetViewBagData();
            using (var client = new HttpClient())
            {
                builder.Path = "/api/product/adminProducts";

                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                if (JsonConvert.DeserializeObject<List<Product>>(content) == null)
                {
                    return HttpNotFound();
                }
                return View(JsonConvert.DeserializeObject<List<Product>>(content));
            }

            //return View(db.Products.ToList());
         
        }

        public ActionResult Create()
        {
            System.Diagnostics.Debug.WriteLine("Hello");
            GetViewBagData();
            return View();
            
        }
        public void GetViewBagData()
        {
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            ViewBag.SubCategoryID = new SelectList(db.SubCategories, "SubCategoryID", "Name");

           
        }

        [HttpPost]
        public async Task<ActionResult> Create(Product prod)
        {

            using (var client = new HttpClient()) {
                if (ModelState.IsValid)
                {
                    //foreach (var file in Picture1)
                    //{
                    //    if (file != null || file.ContentLength > 0)
                    //    {
                    //        string ext = System.IO.Path.GetExtension(file.FileName);
                    //        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                    //        {
                    //            file.SaveAs(Path.Combine(Server.MapPath("/Content/Images/large"), Guid.NewGuid() + Path.GetExtension(file.FileName)));

                    //            var medImg= Images.ResizeImage(Image.FromFile(file.FileName), 250, 300);
                    //            medImg.Save(Path.Combine(Server.MapPath("/Content/Images/medium"), Guid.NewGuid() + Path.GetExtension(file.FileName)));


                    //            var smImg = Images.ResizeImage(Image.FromFile(file.FileName), 45, 55);
                    //            smImg.Save(Path.Combine(Server.MapPath("/Content/Images/small"), Guid.NewGuid() + Path.GetExtension(file.FileName)));

                    //        }
                    //    }
                    //    db.Products.Add(prod);
                    //    db.SaveChanges();
                    //    return RedirectToAction("Index", "Product");
                    //}
                    builder.Path = "/api/product/createProduct";
                    System.Diagnostics.Debug.WriteLine("1");
                    var response = await client.PostAsync(builder.Uri, new StringContent(JsonConvert.SerializeObject(prod), System.Text.Encoding.UTF8, "application/json"));

                    System.Diagnostics.Debug.WriteLine("2");
                    return RedirectToAction("Index", "Product");
                }
                GetViewBagData();
                return View();
            }
                
        }


        //Get Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Single(x => x.ProductID == id);
            if (product == null)
            {

                return HttpNotFound();
            }
            GetViewBagData();
            return View("Edit", product);
        }

        //Post Edit
        [HttpPost]
        public async Task<ActionResult> Edit(Product prod)
        {

            using (var client = new HttpClient())
            {
                if (ModelState.IsValid)
                {
                    builder.Path = "/api/product/editProduct";
                    System.Diagnostics.Debug.WriteLine("1");
                    var response = await client.PutAsync(builder.Uri, new StringContent(JsonConvert.SerializeObject(prod), System.Text.Encoding.UTF8, "application/json"));
                    return RedirectToAction("Index", "Product");
                }
                GetViewBagData();
                return View(prod);
            }
                
        }

        //Get Details
        public async Task<ActionResult> Details(int id)
        {
           
            using (var client = new HttpClient())
            {
                GetViewBagData();
                builder.Path = "/api/product/admindetails/";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["id"] = id.ToString();
                builder.Query = query.ToString();
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                Product product = JsonConvert.DeserializeObject<Product>(content);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
                
        }

        //Get Delete
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);

        }

        //Post Delete Confirmed
        
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/product/delete/";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["id"] = id.ToString();
                builder.Query = query.ToString();

                var response = await client.DeleteAsync(builder.Uri);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
    }
}
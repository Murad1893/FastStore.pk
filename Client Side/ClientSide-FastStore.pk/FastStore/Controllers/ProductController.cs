using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastStore.Models;
using PagedList;
using PagedList.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FastStore.Controllers
{
  public class ProductController : Controller
  {
        UriBuilder builder = new UriBuilder(ConfigurationManager.AppSettings["url"]);
        FastStoreEntities db = new FastStoreEntities();

    public async Task<ActionResult> Index()
    {
        using (var client = new HttpClient())
        {

            ViewBag.Categories = await Categories();

            ViewBag.TopRatedProducts = await TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();

            await this.GetDefaultData();
            return View("Products");
        }        
    }

        // CATGORIES
        public async Task<IEnumerable<String>> Categories() {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/product/categoryNames";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<String>>(content);
            }
        }

        //TOP SOLD PRODUCTS
        public async Task<List<TopSoldProduct>> TopSoldProducts()
        {
            using (var client = new HttpClient())
            {
                builder.Path = "/api/product/soldCount";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TopSoldProduct>>(content);
            }
        }

        //RECENT VIEWS PRODUCTS
        public IEnumerable<Product> RecentViewProducts()
    {
      if (TempShpData.UserID > 0)
      {
        var top3Products = (from recent in db.RecentlyViews
                            where recent.CustomerID == TempShpData.UserID
                            orderby recent.ViewDate descending
                            select recent.ProductID).ToList().Take(3);

        var recentViewProd = db.Products.Where(x => top3Products.Contains(x.ProductID));
        return recentViewProd;
      }
      else
      {
        var prod = (from p in db.Products
                    select p).OrderByDescending(x => x.UnitPrice).Take(3).ToList();
        return prod;
      }
    }

    //ADD TO CART
    public async Task<ActionResult> AddToCart(int id)
    {
        using (var client = new HttpClient())
        {
            OrderDetail OD = new OrderDetail();
            OD.ProductID = id;
            int Qty = 1;

            builder.Path = "/api/product/";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["id"] = id.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();

            Product product = JsonConvert.DeserializeObject<Product>(content);
            decimal price = product.UnitPrice;

            OD.Quantity = Qty;
            OD.UnitPrice = price;
            OD.TotalAmount = Qty * price;

            OD.Product = product;

            if (TempShpData.items == null)
            {
                TempShpData.items = new List<OrderDetail>();
            }
            TempShpData.items.Add(OD);
            AddRecentViewProduct(id);
            return Redirect(TempData["returnURL"].ToString());
        }
    }

    //VIEW DETAILS
    public async Task<ActionResult> ViewDetails(int id)
    {
        using (var client = new HttpClient())
        {
            builder.Path = "/api/product/details/";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["id"] = id.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();

            ArrayList data = JsonConvert.DeserializeObject<ArrayList>(content);

            Product prod = JsonConvert.DeserializeObject<Product>(data[0].ToString());
            var reviews = prod.Reviews;
            ViewBag.Reviews = reviews;
            ViewBag.TotalReviews = reviews.Count();
            var relatedProducts = JArray.FromObject(data[1]);
            ViewBag.RelatedProducts = relatedProducts.ToObject<List<Product>>();
            AddRecentViewProduct(id);

            var ratedProd = reviews;
            int count = ratedProd.Count();
            int TotalRate = ratedProd.Sum(x => x.Rate).GetValueOrDefault();
            ViewBag.AvgRate = TotalRate > 0 ? TotalRate / count : 0;

            await this.GetDefaultData();

            builder.Path = "api/product/categories";
            response = await client.GetAsync(builder.Uri);
            content = await response.Content.ReadAsStringAsync();
            ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

            return View(prod);
        }
                
    }

    //WISHLIST
    public async Task<ActionResult> WishList(int id)
    {
        using (var client = new HttpClient())
        {
            Wishlist wl = new Wishlist();
            wl.ProductID = id;
            wl.CustomerID = TempShpData.UserID;

            builder.Path = "/api/wishlist";
            var postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(wl), Encoding.UTF8, "application/json"));
            AddRecentViewProduct(id);

            builder.Path = "/api/wishlist";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["customerId"] = TempShpData.UserID.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();

            ViewBag.WlItemsNo = JsonConvert.DeserializeObject<IEnumerable<Wishlist>>(content).Count();
            if (TempData["returnURL"].ToString() == "/")
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(TempData["returnURL"].ToString());
        }
               
    }

    //ADD RECENT VIEWS PRODUCT IN DB
    public void AddRecentViewProduct(int pid)
    {
      if (TempShpData.UserID > 0)
      {
        RecentlyView Rv = new RecentlyView();
        Rv.CustomerID = TempShpData.UserID;
        Rv.ProductID = pid;
        Rv.ViewDate = DateTime.Now;
        db.RecentlyViews.Add(Rv);
        db.SaveChanges();
      }
    }

    //ADD REVIEWS ABOUT PRODUCT
    public async Task<ActionResult> AddReview(int productID, FormCollection getReview)
    {
        using (var client = new HttpClient())
        {
            Review r = new Review();
            r.CustomerID = TempShpData.UserID;
            r.ProductID = productID;
            r.Name = getReview["name"];
            r.Email = getReview["email"];
            r.Review1 = getReview["message"];
            r.Rate = Convert.ToInt32(getReview["rate"]);
            r.DateTime = DateTime.Now;

            builder.Path = "/api/product/review";
            var postTask = await client.PostAsync(builder.Uri.ToString(), new StringContent(JsonConvert.SerializeObject(r), Encoding.UTF8, "application/json"));
            return RedirectToAction("ViewDetails/" + productID + "");
        }   
    }


    public async Task<ActionResult> Products(int subCatID)
    {
        using (var client = new HttpClient())
        {
            ViewBag.Categories = await Categories();

            builder.Path = "/api/product/subcategory";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["subcategoryId"] = subCatID.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();

            var prods = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

            await this.GetDefaultData();
            return View(prods);
        }
    }

    //GET PRODUCTS BY CATEGORY
    public async Task<ActionResult> GetProductsByCategory(string categoryName, int? page)
    {
        using (var client = new HttpClient())
        {
            ViewBag.Categories = await Categories();

            ViewBag.TopRatedProducts = await TopSoldProducts() ;

            ViewBag.RecentViewsProducts = RecentViewProducts();

            builder.Path = "/api/product/category";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["categoryName"] = categoryName.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();
            var prods = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

            await this.GetDefaultData();

            builder.Path = "api/product/categories";
            response = await client.GetAsync(builder.Uri);
            content = await response.Content.ReadAsStringAsync();
            ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

            return View("Products", prods.ToPagedList(page ?? 1, 6));
        }
    }

    //SEARCH BAR
    public async Task<ActionResult> Search(string product, int? page)
    {
        using (var client = new HttpClient())
        {
            ViewBag.Categories = await Categories();

            ViewBag.TopRatedProducts = await TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();
            ViewBag.inSearch = true;

            List<Product> products;
            if (!string.IsNullOrEmpty(product))
            {
                builder.Path = "/api/product/search";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["name"] = product.ToString();
                builder.Query = query.ToString();
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();

                products = JsonConvert.DeserializeObject<IEnumerable<Product>>(content).ToList();
                }
            else
            {
                builder.Path = "/api/product/";
                var response = await client.GetAsync(builder.Uri);
                string content = await response.Content.ReadAsStringAsync();
                
                products = JsonConvert.DeserializeObject<IEnumerable<Product>>(content).ToList();
            }

            await this.GetDefaultData();

            builder.Path = "api/product/categories";
            var response1 = await client.GetAsync(builder.Uri);
            string content1 = await response1.Content.ReadAsStringAsync();
            ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content1);

            return View("Products", products.ToPagedList(page ?? 1, 6));
        } 
    }

    public async Task<JsonResult> GetProducts(string term)
    {
        using (var client = new HttpClient())
        {
            builder.Path = "/api/product/search";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["prefix"] = term.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();
            List<string> prodNames = JsonConvert.DeserializeObject<IEnumerable<String>>(content).ToList();
            return Json(prodNames, JsonRequestBehavior.AllowGet);
        }
                
    }
    public async Task<ActionResult> FilterByPrice(int minPrice, int maxPrice, int? page)
    {
        using (var client = new HttpClient())
        {
            ViewBag.Categories = await Categories();
            ViewBag.TopRatedProducts = await TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();
            ViewBag.filterByPrice = true;

            builder.Path = "/api/product/filter";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["minPrice"] = minPrice.ToString();
            query["maxPrice"] = maxPrice.ToString();
            builder.Query = query.ToString();
            var response = await client.GetAsync(builder.Uri);
            string content = await response.Content.ReadAsStringAsync();

            var filterProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(content).ToList();

            await this.GetDefaultData();

            builder.Path = "api/product/categories";
            response = await client.GetAsync(builder.Uri);
            content = await response.Content.ReadAsStringAsync();
            ViewBag.AllCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);

            return View("Products", filterProducts.ToPagedList(page ?? 1, 6));
        }   
    }


  }
}
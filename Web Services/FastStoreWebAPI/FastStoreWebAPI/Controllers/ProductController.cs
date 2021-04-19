using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FastStoreWebAPI.Models;
using Newtonsoft.Json;

namespace FastStoreWebAPI.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        [Route("categories")]
        [HttpGet]
        public IEnumerable<String> GetCategoryNames()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Categories.Select(x => x.Name).ToList();
            }
        }

        [Route("soldCount")]
        [HttpGet]
        public HttpResponseMessage GetTopProductList() {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                var prodList = (from prod in entities.OrderDetails
                                select new { prod.ProductID, prod.Quantity } into p
                                group p by p.ProductID into g
                                select new
                                {
                                    pID = g.Key,
                                    sold = g.Sum(x => x.Quantity)
                                }).OrderByDescending(y => y.sold).Take(3).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, prodList);
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetProductById(int id) {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                var entity = entities.Products.Find(id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Found, entity);
                }
                else {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format("Product with id = {0} not present", id));
                }
            }
        }

        [Route("category/{category}")]
        [HttpGet]
        public IEnumerable<Product> GetProductByCategory(string category)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(x => x.Category.Name == category).ToList();
            }
        }

        [Route("subcategory/{subcategoryId:int}")]
        [HttpGet]
        public IEnumerable<Product> GetProductBySubCategory(int subcategoryId)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(y => y.SubCategoryID == subcategoryId).ToList();
            }
        }

        [Route("details/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetProductDetails(int id) {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                var prod = entities.Products.Find(id);
                ArrayList data = new ArrayList();
                if (prod != null)
                {
                    var reviews = entities.Reviews.Where(x => x.ProductID == id).ToList();
                    var relatedProducts = entities.Products.Where(y => y.CategoryID == prod.CategoryID).ToList();
                    data.Add(prod);
                    data.Add(relatedProducts);
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format("Product with id = {0} not present", id));
                }
            }
        }

        [Route("review")]
        [HttpPost]
        public HttpResponseMessage AddReview([FromBody] Review r) {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.Reviews.Add(r);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Review added.");
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("wishlist")]
        [HttpPost]
        public HttpResponseMessage AddToWishList([FromBody] Wishlist w)
        {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.Wishlists.Add(w);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Item added to wishlist");
                }
            }
            catch (Exception e) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("wishlist/{id:int}")]
        [HttpGet]
        public IEnumerable<Wishlist> GetWishList(int id)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Wishlists.Where(x => x.CustomerID == id).ToList();
            }
        }

        [Route("search/{name}")]
        [HttpGet]
        public IEnumerable<String> GetProductNames(string name) {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(x => x.Name.StartsWith(name)).Select(y => y.Name).ToList();
            }
        }

        [Route("filter")]
        [HttpGet]
        public IEnumerable<Product> FilterProductByPrice(int minPrice, int maxPrice) {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice).ToList();
            }
        }

    }
}

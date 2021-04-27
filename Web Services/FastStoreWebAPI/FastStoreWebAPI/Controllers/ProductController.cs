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
        public IEnumerable<TopSoldProduct> GetTopProductList() {
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

                List<TopSoldProduct> topSoldProds = new List<TopSoldProduct>();
                for (int i = 0; i < 3; i++)
                {
                    topSoldProds.Add(new TopSoldProduct()
                    {
                        product = entities.Products.Find(prodList[i].pID),
                        CountSold = Convert.ToInt32(prodList[i].sold)
                    });
                }

                return topSoldProds;
            }
        }

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

        [Route("category")]
        [HttpGet]
        public IEnumerable<Product> GetProductByCategory(string categoryName)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(x => x.Category.Name == categoryName).ToList();
            }
        }

        [Route("subcategory")]
        [HttpGet]
        public IEnumerable<Product> GetProductBySubCategory(int subcategoryId)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Products.Where(y => y.SubCategoryID == subcategoryId).ToList();
            }
        }

        [Route("details")]
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

        [Route("search")]
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

        [Route("~/api/genMainSliders")]
        [HttpGet]
        public IEnumerable<genMainSlider> GetSlider() {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.genMainSliders.ToList();
            }
        }
    }
}

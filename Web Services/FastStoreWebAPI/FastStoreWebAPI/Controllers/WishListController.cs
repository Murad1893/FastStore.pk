using FastStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FastStoreWebAPI.Controllers
{
    [RoutePrefix("api/wishlist")]
    public class WishListController : ApiController
    {
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
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [HttpGet]
        public Wishlist GetWishList(int id)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                Wishlist w = entities.Wishlists.Find(id);
                w.Product = entities.Products.Find(w.ProductID);

                return w;
            }
        }

        [HttpGet]
        public IEnumerable<Wishlist> GetCustomerWishList(int customerId)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                IEnumerable<Wishlist> entity = entities.Wishlists.Where(x => x.CustomerID == customerId).ToList();
                foreach (Wishlist item in entity) {
                    item.Product = entities.Products.Find(item.ProductID);
                }

                return entity;
            }
        }

        [HttpDelete]
        public HttpResponseMessage RemoveFromWishList(int id) {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    // check if item present
                    var entity = entities.Wishlists.Find(id);

                    if (entity != null)
                    {
                        entities.Wishlists.Remove(entity);
                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, "Item removed wishlist");
                    }
                    else {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not present in wish list");
                    }
                    
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}

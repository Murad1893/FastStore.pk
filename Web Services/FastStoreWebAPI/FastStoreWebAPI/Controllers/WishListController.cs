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
        public IEnumerable<Wishlist> GetCustomerWishList(int customerId)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Wishlists.Where(x => x.CustomerID == customerId).ToList();
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

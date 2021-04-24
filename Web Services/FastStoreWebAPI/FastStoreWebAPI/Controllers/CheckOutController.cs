using FastStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FastStoreWebAPI.Controllers
{
    [RoutePrefix("api/checkout")]
    public class CheckOutController : ApiController
    {
        [Route("paymentTypes")]
        [HttpGet]
        public dynamic GetPaymentTypes()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.PaymentTypes.Select(p => new { p.PayTypeID, p.TypeName }).ToList();
            }
        }

        [Route("nextShippingID")]
        [HttpGet]
        public int GetNextShippingID() {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.ShippingDetails.Max(x => x.ShippingID) + 1;
            }
        }

        [Route("nextPaymentID")]
        [HttpGet]
        public int GetNextPaymentID()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Payments.Max(x => x.PaymentID) + 1;
            }
        }

        [Route("nextOrderID")]
        [HttpGet]
        public int GetNextOrderID()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Orders.Max(x => x.OrderID) + 1;
            }
        }

        [Route("shippingDetails")]
        [HttpPost]
        public HttpResponseMessage AddShippingDetails([FromBody] ShippingDetail shpDetails) {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.ShippingDetails.Add(shpDetails);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Added shipping details");
                }
            }
            catch (Exception e) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("payments")]
        [HttpPost]
        public HttpResponseMessage AddPayment([FromBody] Payment pay)
        {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.Payments.Add(pay);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Added payment details");
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("orders")]
        [HttpPost]
        public HttpResponseMessage AddOrder([FromBody] Order o)
        {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.Orders.Add(o);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Added order");
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("orderDetails")]
        [HttpPost]
        public HttpResponseMessage AddOrderDetails([FromBody] OrderDetail OD)
        {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    entities.OrderDetails.Add(OD);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Added order details for order ID: " + OD.OrderID);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [Route("~/api/orders")]
        [HttpGet]
        public IEnumerable<Order> GetOrder(int orderID)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Orders.Where(x => x.OrderID == orderID).ToList();
            }
        }

    }
}

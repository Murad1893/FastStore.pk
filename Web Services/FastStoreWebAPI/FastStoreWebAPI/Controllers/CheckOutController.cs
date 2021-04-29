using FastStoreWebAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
        public IEnumerable<PaymentType> GetPaymentTypes()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.PaymentTypes.ToList();
            }
        }

        [Route("nextShippingID")]
        [HttpGet]
        public HttpResponseMessage GetNextShippingID() {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                if (entities.ShippingDetails.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.ShippingDetails.Max(x => x.ShippingID) + 1);
                }
                else {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No shipping entries found");
                }
                
            }
        }

        [Route("nextPaymentID")]
        [HttpGet]
        public HttpResponseMessage GetNextPaymentID()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                if (entities.Payments.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.Payments.Max(x => x.PaymentID) + 1);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No payment entries found");
                }
            }
        }

        [Route("nextOrderID")]
        [HttpGet]
        public HttpResponseMessage GetNextOrderID()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                if (entities.Orders.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.Orders.Max(x => x.OrderID) + 1);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No order entries found");
                }
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
        public Order GetOrder(int orderID)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Orders.Find(orderID);
            }
        }

    }
}

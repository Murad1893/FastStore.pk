using FastStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Newtonsoft.Json;

namespace FastStoreWebAPI.Controllers
{
    public class AccountController : ApiController
    {
        // GET All Customers: account/
        [Route("~/api/customers")]
        [HttpGet]
        public IEnumerable<Customer> GetCustomerList()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Customers.ToList();
            }
        }

        // GET Specific Customer: account/:id 
        [Route("~/api/customers")]
        [HttpGet]
        public HttpResponseMessage GetCustomerById(int id) {
            using (EcommerceEntities entities = new EcommerceEntities()) {
                var entity = entities.Customers.FirstOrDefault(e => e.CustomerID == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer with id " + id.ToString() + " not found");
                }
            }
        }

        // GET Specific Customer: account/:username 
        [Route("~/api/customers")]
        [HttpGet]
        public HttpResponseMessage GetCustomerbyUsername(string username)
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                var entity = entities.Customers.FirstOrDefault(e => e.UserName == username);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer with username " + username + " not found");
                }
            }
        }

        // POST Customer: account/
        [Route("~/api/customers")]
        [HttpPost]
        public HttpResponseMessage RegisterCustomer([FromBody] Customer customer) {
            try
            {
                using (EcommerceEntities entities = new EcommerceEntities())
                {
                    var entity = entities.Customers.FirstOrDefault(e => e.UserName == customer.UserName);

                    // if customer with same username not present then add user
                    if (entity == null) 
                    {
                        entities.Customers.Add(customer);
                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.Created, "Customer Registered Successfully");
                    }
                    else {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "A customer with the same username already exists.");
                    } 
                }
            }
            catch (Exception e) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to register customer");
            }
        }

        // Updating customer details : account/?id
        [Route("~/api/customers")]
        [HttpPut]
        public HttpResponseMessage UpdateCustomer(int id, [FromBody] Customer customer) {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                try
                {
                    // updating relevant customer fields on put
                    entities.Entry(customer).State = EntityState.Modified;
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Customer updated successfully");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e) {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter all non-nullable fields for update validation");
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException e) {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer id is null or is not present in database");
                }
            }
        }

    }
}

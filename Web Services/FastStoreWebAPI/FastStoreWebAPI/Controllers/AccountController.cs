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
        public IEnumerable<Customer> Get()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Customers.ToList();
            }
        }

        // GET Specific Customer: account/:id 
        public Customer Get(int id) {
            using (EcommerceEntities entities = new EcommerceEntities()) {
                return entities.Customers.FirstOrDefault(e => e.CustomerID == id);
            }
        }

    }
}

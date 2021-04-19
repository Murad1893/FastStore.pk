using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FastStoreWebAPI.Models;

namespace FastStoreWebAPI.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        [Route("categories")]
        [HttpGet]
        public IEnumerable<String> ListCategories()
        {
            using (EcommerceEntities entities = new EcommerceEntities())
            {
                return entities.Categories.Select(x => x.Name).ToList();
            }
        }
    }
}

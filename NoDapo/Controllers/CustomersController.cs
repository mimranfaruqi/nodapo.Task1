using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NoDapo.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CustomersController : Controller
    {
        private readonly Data _data;

        public CustomersController(Data data)
        {
            _data = data;
        }
        
        [HttpGet]
        public IActionResult Customers()
        {
            var customers = _data.Customers.ToList();

            if (customers.Count == 0) return NotFound();
            
            return Ok(_data.Customers.ToList());
        }
    }
}
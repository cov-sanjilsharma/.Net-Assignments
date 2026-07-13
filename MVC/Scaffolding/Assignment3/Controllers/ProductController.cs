using Assignment3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3.Controllers
{
    public class ProductController : Controller
    {
        public List<Product> products = new List<Product>
        {
            new Product(1,"Clothes", 500),
            new Product(2,"Utensils", 304)

        };
        public IActionResult Index()
        {
            return View(products);
        }
        //[HttpGet("Product/Details/{id}")]
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult ProductDetails(int id)
        {
            var product = FindProductById(id);
            if (product == null) return NotFound("Product not found");
            return View("ViewDetails", product);
        }

        [HttpPost]
        public IActionResult Details(int id)
        {
            var product = FindProductById(id);
            if (product == null) return NotFound("Product not found");
            return View("ViewDetails", product);
        }


        [NonAction]
        public Product FindProductById(int id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }
    }
}

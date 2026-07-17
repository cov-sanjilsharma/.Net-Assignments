using Microsoft.AspNetCore.Mvc;
using Ecommerce_CodeFirst_Demo.Models;

namespace Ecommerce_CodeFirst_Demo.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            var product = _appDbContext.Products.ToList();
            
            return View(product);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Products.Add(product);
                _appDbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }
    }
}
